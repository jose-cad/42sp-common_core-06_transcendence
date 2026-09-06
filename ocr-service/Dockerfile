# Exemplo/ponto de partida — ajustar quando o projeto FastAPI existir de verdade.
# Assume um requirements.txt e um main.py com "app = FastAPI()".

FROM python:3.12-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt
COPY . .
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "8000"]
