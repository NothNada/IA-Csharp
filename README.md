# 🧠 Rede Neural Genética em C#

Este projeto implementa uma **Rede Neural** com evolução genética, escrita em **C#**.  
A lógica da rede está encapsulada na classe `RedeNeural.cs`, que contém todos os métodos necessários para criação, avaliação e evolução dos indivíduos.

O executável **`teste.exe`** está incluso e pode ser executado diretamente em ambientes Windows.

---

## 🎮 Controles do Programa

Durante a execução, você pode interagir com as seguintes teclas:

| Tecla | Ação |
|------|------|
| `ESC` | Encerra o programa |
| `C` | Limpa a tela e continua a execução |
| `Q` | Avança para a próxima geração |
| `P` | Busca o melhor indivíduo atual |
| `D` | Pula 10 execuções automaticamente |
| `S` | Salva o indivíduo atual no arquivo `MelhorIndividuo.bin` |
| `A` | Carrega o indivíduo salvo de `MelhorIndividuo.bin` para o cenário atual |
| `L` | Exibe os pesos de cada camada da rede neural (log detalhado) |

---
