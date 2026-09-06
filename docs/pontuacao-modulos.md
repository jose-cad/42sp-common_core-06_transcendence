# Como funciona a pontuação de módulos

## Regras do subject
- Mínimo para passar: **14 pontos**.
- **Major = 2 pontos**, **Minor = 1 ponto**.
- Podem misturar módulos de qualquer categoria, sem limite por categoria.
- Só conta o que for **demonstrado funcionando de verdade** na avaliação. Módulo incompleto ou que ninguém do time sabe explicar = **0 pontos**, mesmo que o código exista.
- Alguns módulos têm pré-requisito (ex.: módulos de jogo exigem um jogo implementado antes) — não é o caso do projeto de vocês, que não tem módulo de Gaming.
- Um "Módulo de escolha" (Web IV.10) só é aceito se o README justificar: por que escolheram, que desafio técnico resolve, como agrega valor, e por que merece o status (major/minor).

## Meta do time: 125%
14 × 1,25 = **17,5** → para garantir, mirem em **18 pontos**.

## Plano completo para 18 pontos (129%)

| # | Categoria | Módulo | Tipo | Pontos | Por que se encaixa |
|---|---|---|---|---|---|
| 1 | Web | Framework frontend + backend (React + ASP.NET Core) | Major | 2 | Os dois frameworks usados em capacidade completa |
| 2 | Web | API pública documentada (API key + rate limit, 5+ endpoints) | Major | 2 | CRUD de motoristas/carros exposto como API REST |
| 3 | Web | Upload e gestão de arquivos | Minor | 1 | CNH, contrato, CRV com validação/preview/exclusão |
| 4 | Web | Real-time via WebSocket | Major | 2 | SignalR propagando alertas a todos os usuários conectados |
| 5 | User Management | Permissões avançadas (admin/gestor/operador) | Major | 2 | Views e ações diferentes por papel |
| 6 | User Management | Sistema de organizações (multi-tenant) | Major | 2 | Cada locadora isolada dentro do SaaS |
| 7 | Data and Analytics | Dashboard de analytics avançado | Major | 2 | Gráficos de km, manutenção e alertas |
| 8 | Artificial Intelligence | Reconhecimento de imagem (OCR) | Minor | 1 | Extração de texto de fotos via Gemini |
| 9 | Devops | Backend como microsserviços | Major | 2 | 3 serviços (.NET, Java, Python) via RabbitMQ |
| 10 | **Modules of choice** | **Verificação automática de Pix via e-mail** | **Major** | **2** | Não existe API bancária gratuita pra isso — pipeline próprio (OCR do comprovante + IMAP + regra de conferência) resolve um problema real do negócio |
| | | | **Total** | **18** | **129% da exigência** |

## Justificativa do módulo #10 pro README
Esse é o único que exige texto extra (os outros 9 são módulos padrão da lista, não precisam de justificativa). No README, em "Modules", detalhem:
- **Por que escolheram**: não existe alternativa gratuita pronta (Open Finance/PIX não oferece confirmação de recebimento via API pública gratuita).
- **Desafio técnico**: orquestrar 2 fontes de verdade diferentes (texto extraído por IA + e-mail real do banco) e decidir "confirmado" vs "divergente" com tolerância de valor/data.
- **Valor agregado**: automatiza uma tarefa manual (funcionário conferindo e-mail um por um) que é o motivador original do projeto.
- **Por que é Major**: envolve 2 integrações externas (Gemini + IMAP), lógica de reconciliação, e trata falhas (e-mail não chega, valor não bate) com o alerta `pix_divergente`.

## Se quiserem colchão extra além de 125%
Nenhum destes é necessário pra bater a meta — só listo caso quatro tarefas pequenas ajudem a distribuir risco entre os integrantes:

| Módulo | Tipo | Pontos | Esforço |
|---|---|---|---|
| OAuth (Google) | Minor | 1 | Baixo — ASP.NET Identity já suporta |
| 2FA | Minor | 1 | Baixo/médio |
| GDPR/LGPD (exportar/apagar dados) | Minor | 1 | Baixo — estende a Privacy Policy já obrigatória |
| i18n (PT/EN/ES) | Minor | 1 | Médio — trabalho manual de tradução |

Cada um a mais é código, teste e explicação extra na avaliação — com 5 serviços em 3 linguagens já é bastante superfície. O plano de 18 pontos acima já cobre os 125% sem precisar de nenhum destes.
