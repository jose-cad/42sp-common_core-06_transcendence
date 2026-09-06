# Contrato de mensagens (RabbitMQ)

Três linguagens diferentes (.NET, Java, Python) trocam eventos por uma fila em vez de se chamarem diretamente por HTTP. Formato: **JSON**, sempre com o mesmo envelope.

## Por que mensageria e não chamada direta

- O chatbot (Java) não pode travar esperando o Gemini responder (o serviço de OCR pode demorar, falhar, ou estar reiniciando).
- Cada serviço escala e é implantado de forma independente — se o serviço de Pix cair, o chatbot continua respondendo o resto do menu normalmente.
- Usamos o padrão **RPC sobre fila**: quem pergunta gera um `correlation_id`, publica numa fila de request e escuta uma fila de resposta (com timeout). Isso dá a sensação de "chamei e recebi a resposta" pro usuário do chat, mas sem acoplamento direto entre os serviços.

Nem tudo precisa passar pela fila: mudanças que só acontecem dentro do próprio .NET (ex.: gravar no banco depois de validar um formulário) são chamada de função normal — a fila é só para comunicação **entre** os três serviços.

## Envelope padrão

```json
{
  "evento": "nome.do.evento",
  "correlation_id": "uuid-v4",
  "origem": "motorista | carro",
  "payload": { },
  "timestamp": "2026-09-04T13:00:00Z"
}
```

- `correlation_id`: gerado no início do fluxo (quando o usuário manda a foto no chat) e repetido em toda a cadeia de eventos daquela interação — é o que permite juntar os logs de ponta a ponta.
- `origem`: diz se o fluxo é sobre motorista ou carro. Importante porque o serviço de OCR é **genérico** (imagem entra, texto sai) — ele não sabe nem precisa saber o que está processando. Quem decide o que fazer com o texto é sempre o chatbot.

## Exchange e filas (RabbitMQ)

- Exchange do tipo `topic`: `transcendence.eventos`
- Routing key = nome do evento (ex.: `ocr.texto.extraido`)
- Uma fila por serviço consumidor, com binding só nos eventos que ele escuta

| Fila | Consome | Serviço |
|---|---|---|
| `q.ocr.requisicoes` | `chat.imagem.recebida` | OCR (Python) |
| `q.chatbot.respostas` | `ocr.texto.extraido`, `ocr.falha`, `pix.confirmado`, `pix.divergente` | Chatbot (Java) |
| `q.pix.requisicoes` | `pix.verificar` | Pix (Python) |
| `q.backend.eventos` | `carro.km.atualizar`, `pix.confirmado`, `pix.divergente` | Backend (.NET) |

## Eventos

### `chat.imagem.recebida`
Chatbot salva a imagem recebida no volume compartilhado `/storage/tmp/` e publica a referência (nunca a imagem em base64 dentro da mensagem — mantém a fila leve).

```json
{
  "evento": "chat.imagem.recebida",
  "correlation_id": "c290f1ee-6c54-4b01-90e6-d701748f0851",
  "origem": "carro",
  "payload": {
    "arquivo_hash": "a1b2c3d4",
    "arquivo_path": "/storage/tmp/a1b2c3d4.jpg",
    "contexto_esperado": "km"
  },
  "timestamp": "2026-09-04T13:00:00Z"
}
```
`contexto_esperado` pode ser `"km"`, `"pix"` ou `"documento"` — é o que o chatbot já sabe pelo menu que o usuário escolheu antes de mandar a foto.

### `ocr.texto.extraido`
Publicado pelo serviço Python após a chamada ao Gemini.

```json
{
  "evento": "ocr.texto.extraido",
  "correlation_id": "c290f1ee-6c54-4b01-90e6-d701748f0851",
  "origem": "carro",
  "payload": {
    "arquivo_hash": "a1b2c3d4",
    "texto_extraido": "45210 km",
    "confianca": 0.94
  },
  "timestamp": "2026-09-04T13:00:04Z"
}
```

### `ocr.falha`
Quando o Gemini não retorna nada legível (foto borrada, etc.) — o chatbot usa isso pra pedir uma nova foto ao usuário em vez de travar o fluxo.

```json
{
  "evento": "ocr.falha",
  "correlation_id": "c290f1ee-6c54-4b01-90e6-d701748f0851",
  "payload": { "arquivo_hash": "a1b2c3d4", "motivo": "texto_nao_reconhecido" },
  "timestamp": "2026-09-04T13:00:04Z"
}
```

### `carro.km.atualizar`
Publicado pelo chatbot depois de interpretar o texto extraído no contexto `"km"`. Consumido pelo backend .NET, que faz a regra de negócio (comparar com km da semana anterior, disparar alerta se > 1250km, checar prazo de manutenção).

```json
{
  "evento": "carro.km.atualizar",
  "correlation_id": "c290f1ee-6c54-4b01-90e6-d701748f0851",
  "payload": { "placa": "ABC1D23", "km_lido": 45210 },
  "timestamp": "2026-09-04T13:00:05Z"
}
```

### `pix.verificar`
Publicado pelo chatbot no contexto `"pix"`. Consumido pelo serviço Python que faz IMAP na caixa de e-mail da empresa.

```json
{
  "evento": "pix.verificar",
  "correlation_id": "c290f1ee-6c54-4b01-90e6-d701748f0851",
  "payload": {
    "motorista_id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
    "valor_extraido": 850.00,
    "data_extraida": "2026-09-04"
  },
  "timestamp": "2026-09-04T13:00:05Z"
}
```

### `pix.confirmado` / `pix.divergente`
Resposta do serviço de Pix após conferir o e-mail. Consumida pelo backend (grava o pagamento) e pelo chatbot (avisa o usuário).

```json
{
  "evento": "pix.confirmado",
  "correlation_id": "c290f1ee-6c54-4b01-90e6-d701748f0851",
  "payload": {
    "motorista_id": "3f2504e0-4f89-11d3-9a0c-0305e82c3301",
    "valor": 850.00,
    "email_referencia_id": "<msgid@banco.com>"
  },
  "timestamp": "2026-09-04T13:00:09Z"
}
```

## Alertas em tempo real

`alerta.criado` **não precisa passar pela fila** — é gerado dentro do próprio backend .NET (depois de consumir `carro.km.atualizar`, por exemplo) e enviado direto aos clientes conectados via **SignalR** (WebSocket), além de gravado na tabela `alertas`. A fila serve pra comunicação entre linguagens/serviços diferentes; dentro do próprio .NET, é só uma chamada de função.
