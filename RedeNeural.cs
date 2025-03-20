using System;
using System.IO;

public static class Aleatorio{
    private static Random rd = new Random();
    public static int Inteiro(){
        return rd.Next();
    }
    public static double GetRandomValue(){
        return (rd.Next(20001) / 10.0) - 1000.0;
    }
}

public class Neuronio{
    public double[] Peso { get; set; }
    public double Erro { get; set; }
    public double Saida { get; set; }
    public int QuantidadeLigacoes { get; set; }

    public Neuronio(int quantidadeLigacoes){
        QuantidadeLigacoes = quantidadeLigacoes;
        Peso = new double[quantidadeLigacoes];
        for(int i=0;i<quantidadeLigacoes;i++){
            Peso[i] = Aleatorio.GetRandomValue();
        }
        Erro = 0;
        Saida = 1;
    }
}

public class Camada{
    public Neuronio[] Neuronios { get; set; }
    public int QuantidadeNeuronios { get; set; }

    public Camada(int quantidadeNeuronios){
        QuantidadeNeuronios = quantidadeNeuronios;
        Neuronios = new Neuronio[quantidadeNeuronios];
    }
}

public class RedeNeural{
    public Camada CamadaEntrada { get; set; }
    public Camada[] CamadaEscondida { get; set; }
    public Camada CamadaSaida { get; set; }
    public int QuantidadeEscondidas { get; set; }

    private const double TAXA_APRENDIZADO = 0.1;
    private const double TAXA_PESO_INICIAL = 1.0;
    private const int BIAS = 1;

    public RedeNeural(int quantidadeEscondidas, int qtdNeuroniosEntrada, int qtdNeuroniosEscondida, int qtdNeuroniosSaida){
        qtdNeuroniosEntrada += BIAS;
        qtdNeuroniosEscondida += BIAS;

        CamadaEntrada = new Camada(qtdNeuroniosEntrada);
        for (int i = 0; i < qtdNeuroniosEntrada; i++){
            CamadaEntrada.Neuronios[i] = new Neuronio(0);
        }

        QuantidadeEscondidas = quantidadeEscondidas;
        CamadaEscondida = new Camada[quantidadeEscondidas];
        for (int i = 0; i < quantidadeEscondidas; i++){
            CamadaEscondida[i] = new Camada(qtdNeuroniosEscondida);
            for (int j = 0; j < qtdNeuroniosEscondida; j++){
                if (i == 0){
                    CamadaEscondida[i].Neuronios[j] = new Neuronio(qtdNeuroniosEntrada);
                }else{
                    CamadaEscondida[i].Neuronios[j] = new Neuronio(qtdNeuroniosEscondida);
                }
            }
        }

        CamadaSaida = new Camada(qtdNeuroniosSaida);
        for (int j = 0; j < qtdNeuroniosSaida; j++){
            CamadaSaida.Neuronios[j] = new Neuronio(qtdNeuroniosEscondida);
        }
    }

    public double ReLU(double x){
        if (x < 0)
            return 0;
        return x < 10000 ? x : 10000;
    }

    public void CopiarVetorParaCamadas(double[] vetor){
        int j = 0;

        for (int i = 0; i < QuantidadeEscondidas; i++){
            for (int k = 0; k < CamadaEscondida[i].QuantidadeNeuronios; k++){
                for (int l = 0; l < CamadaEscondida[i].Neuronios[k].QuantidadeLigacoes; l++){
                    CamadaEscondida[i].Neuronios[k].Peso[l] = vetor[j++];
                }
            }
        }

        for (int k = 0; k < CamadaSaida.QuantidadeNeuronios; k++){
            for (int l = 0; l < CamadaSaida.Neuronios[k].QuantidadeLigacoes; l++){
                CamadaSaida.Neuronios[k].Peso[l] = vetor[j++];
            }
        }
    }

    public void CopiarCamadasParaVetor(double[] vetor){
        int j = 0;

        for (int i = 0; i < QuantidadeEscondidas; i++){
            for (int k = 0; k < CamadaEscondida[i].QuantidadeNeuronios; k++){
                for (int l = 0; l < CamadaEscondida[i].Neuronios[k].QuantidadeLigacoes; l++){
                    vetor[j++] = CamadaEscondida[i].Neuronios[k].Peso[l];
                }
            }
        }

        for (int k = 0; k < CamadaSaida.QuantidadeNeuronios; k++){
            for (int l = 0; l < CamadaSaida.Neuronios[k].QuantidadeLigacoes; l++){
                vetor[j++] = CamadaSaida.Neuronios[k].Peso[l];
            }
        }
    }

    public void CopiarParaEntrada(double[] vetorEntrada){
        for (int i = 0; i < CamadaEntrada.QuantidadeNeuronios - BIAS; i++){
            CamadaEntrada.Neuronios[i].Saida = vetorEntrada[i];
        }
    }

    public int QuantidadePesos(){
        int soma = 0;
        for (int i = 0; i < QuantidadeEscondidas; i++){
            for (int j = 0; j < CamadaEscondida[i].QuantidadeNeuronios; j++){
                soma += CamadaEscondida[i].Neuronios[j].QuantidadeLigacoes;
            }
        }

        for (int i = 0; i < CamadaSaida.QuantidadeNeuronios; i++){
            soma += CamadaSaida.Neuronios[i].QuantidadeLigacoes;
        }
        return soma;
    }

    public void CopiarDaSaida(double[] vetorSaida){
        for (int i = 0; i < CamadaSaida.QuantidadeNeuronios; i++){
            vetorSaida[i] = CamadaSaida.Neuronios[i].Saida;
        }
    }

    public void CalcularSaida(){
        double somatorio;

        // Calculando saidas entre a camada de entrada e a primeira camada escondida
        for (int i = 0; i < CamadaEscondida[0].QuantidadeNeuronios - BIAS; i++){
            somatorio = 0;
            for (int j = 0; j < CamadaEntrada.QuantidadeNeuronios; j++){
                somatorio += CamadaEntrada.Neuronios[j].Saida * CamadaEscondida[0].Neuronios[i].Peso[j];
            }
            CamadaEscondida[0].Neuronios[i].Saida = ReLU(somatorio);
        }

        // Calculando saidas entre as camadas escondidas
        for (int k = 1; k < QuantidadeEscondidas; k++){
            for (int i = 0; i < CamadaEscondida[k].QuantidadeNeuronios - BIAS; i++){
                somatorio = 0;
                for (int j = 0; j < CamadaEscondida[k - 1].QuantidadeNeuronios; j++){
                    somatorio += CamadaEscondida[k - 1].Neuronios[j].Saida * CamadaEscondida[k].Neuronios[i].Peso[j];
                }
                CamadaEscondida[k].Neuronios[i].Saida = ReLU(somatorio);
            }
        }

        // Calculando saidas entre a camada de saida e a ultima camada escondida
        for (int i = 0; i < CamadaSaida.QuantidadeNeuronios; i++){
            somatorio = 0;
            for (int j = 0; j < CamadaEscondida[QuantidadeEscondidas - 1].QuantidadeNeuronios; j++){
                somatorio += CamadaEscondida[QuantidadeEscondidas - 1].Neuronios[j].Saida * CamadaSaida.Neuronios[i].Peso[j];
            }
            CamadaSaida.Neuronios[i].Saida = ReLU(somatorio);
        }
    }

    public void ImprimirPesos(){
        for (int k = 0; k < QuantidadeEscondidas; k++){
            Console.WriteLine("Camada escondida " + k);
            for (int i = 0; i < CamadaEscondida[k].QuantidadeNeuronios; i++){
                Console.WriteLine("\tNeuronio " + i);
                for (int j = 0; j < CamadaEscondida[k].Neuronios[i].QuantidadeLigacoes; j++){
                    Console.WriteLine("\t\tPeso " + j + " : " + CamadaEscondida[k].Neuronios[i].Peso[j]);
                }
            }
        }

        Console.WriteLine("Camada saida");
        for (int i = 0; i < CamadaSaida.QuantidadeNeuronios; i++){
            Console.WriteLine("\tNeuronio " + i);
            for (int j = 0; j < CamadaSaida.Neuronios[i].QuantidadeLigacoes; j++){
                Console.WriteLine("\t\tPeso " + j + " : " + CamadaSaida.Neuronios[i].Peso[j]);
            }
        }
    }

    public void SalvarRede(string caminho){
        using (FileStream fs = new FileStream(caminho, FileMode.Create)) using (BinaryWriter writer = new BinaryWriter(fs)){
            writer.Write(QuantidadeEscondidas);
            writer.Write(CamadaEntrada.QuantidadeNeuronios);
            writer.Write(CamadaEscondida[0].QuantidadeNeuronios);
            writer.Write(CamadaSaida.QuantidadeNeuronios);

            for (int k = 0; k < QuantidadeEscondidas; k++){
                for (int i = 0; i < CamadaEscondida[k].QuantidadeNeuronios; i++){
                    for (int j = 0; j < CamadaEscondida[k].Neuronios[i].QuantidadeLigacoes; j++){
                        writer.Write(CamadaEscondida[k].Neuronios[i].Peso[j]);
                    }
                }
            }

            for (int i = 0; i < CamadaSaida.QuantidadeNeuronios; i++){
                for (int j = 0; j < CamadaSaida.Neuronios[i].QuantidadeLigacoes; j++){
                    writer.Write(CamadaSaida.Neuronios[i].Peso[j]);
                }
            }
        }
    }

    public static RedeNeural CarregarRede(string caminho){
        using (FileStream fs = new FileStream(caminho, FileMode.Open)) using (BinaryReader reader = new BinaryReader(fs)){
            int qtdEscondida = reader.ReadInt32();
            int qtdNeuroEntrada = reader.ReadInt32();
            int qtdNeuroEscondida = reader.ReadInt32();
            int qtdNeuroSaida = reader.ReadInt32();

            RedeNeural rede = new RedeNeural(qtdEscondida, qtdNeuroEntrada - BIAS, qtdNeuroEscondida - BIAS, qtdNeuroSaida);

            for (int k = 0; k < rede.QuantidadeEscondidas; k++){
                for (int i = 0; i < rede.CamadaEscondida[k].QuantidadeNeuronios; i++){
                    for (int j = 0; j < rede.CamadaEscondida[k].Neuronios[i].QuantidadeLigacoes; j++){
                        rede.CamadaEscondida[k].Neuronios[i].Peso[j] = reader.ReadDouble();
                    }
                }
            }

            for (int i = 0; i < rede.CamadaSaida.QuantidadeNeuronios; i++){
                for (int j = 0; j < rede.CamadaSaida.Neuronios[i].QuantidadeLigacoes; j++){
                    rede.CamadaSaida.Neuronios[i].Peso[j] = reader.ReadDouble();
                }
            }

            return rede;
        }
    }
}
