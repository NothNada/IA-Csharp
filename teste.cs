using System;

struct IA{
    public int x;
    public int y;
    public bool live;

    public int TamanhoDNA;
    public double Fitness;
    public double[] DNA;

    public RedeNeural rede;
}

struct Obj{
    public int x;
    public int y;

    public Obj(int x,int y){
        this.x = x;
        this.y = y;
    }
}

class Program{
    private static int LARGURA_JANELA = 20;
    private static int ALTURA_JANELA = 20;
    private static readonly int SPEED = 1;

    private static readonly int NUMBER_OF_HIDDEN_LAYERS = 1;
    private static readonly int NUMBER_OF_HIDDEN_NEURONS = 4;
    private static readonly int NUMBER_OF_INPUT_NEURONS = 2;
    private static readonly int NUMBER_OF_OUTPUT_NEURONS = 4;

    private static readonly int POPULACAO = 10000;
    private static double[][] DnaDaVez = new double[POPULACAO][];
    private static IA[] Individuos = new IA[POPULACAO];
    private static IA MelhorIndividuo;
    private static int MelhorIndividuoI;
    private static int mortos = 0;
    private static double RangeRandom;
    private static Random rd = new Random();

    private static bool fimGer = false;

    private static Obj[] Objetos = new Obj[POPULACAO];

    static void Main(string[] args){
        bool fim = false;

        ConsoleKeyInfo tecla;
        AlocarIndividuos();

        for(int i=0;i<POPULACAO;i++){
            Objetos[i] = new Obj(3,3);
        }

        

        while(!fim){

            UpdateMelhor();
            ControlarIndividuos();

            DesenharMapa();
            //Console.Clear();

            tecla = Console.ReadKey();
            if(tecla.Key == ConsoleKey.Escape){
                fim = true;
            }
            if(tecla.KeyChar == 'c'){
                Console.Clear();
            }
            if(tecla.KeyChar == 'q'){
                fimGer = true;
            }

            VerificaFimDePartida();

        }

        // // Salvar a rede em um arquivo
        // string caminhoArquivo = "rede_neural.bin";
        // rede.SalvarRede(caminhoArquivo);
        // Console.WriteLine("Rede neural salva em "+caminhoArquivo);

        // // Carregar a rede de um arquivo
        // RedeNeural redeCarregada = RedeNeural.CarregarRede(caminhoArquivo);
        // Console.WriteLine("Rede neural carregada com sucesso.");
        
        // // Calcular a saída da rede carregada
        // redeCarregada.CopiarParaEntrada(entradas);
        // redeCarregada.CalcularSaida();
        // double[] saidaCarregada = new double[redeCarregada.CamadaSaida.QuantidadeNeuronios];
        // redeCarregada.CopiarDaSaida(saidaCarregada);

        // // Imprimir a saída da rede carregada
        // Console.WriteLine("Saída da rede carregada:");
        // foreach (var valor in saidaCarregada)
        // {
        //     Console.WriteLine(valor);
        // }
    }

    static void RandomMutations(){
        RangeRandom = Individuos[0].TamanhoDNA;

        IA[] Vetor = new IA[POPULACAO];
        IA Temp;

        for(int i=0; i<POPULACAO; i++){
            Vetor[i] = Individuos[i];
        }

        for(int i=0; i<POPULACAO; i++){
            for(int j=0; j<POPULACAO-1; j++){
                if(Vetor[j].Fitness < Vetor[j+1].Fitness){
                    Temp = Vetor[j];
                    Vetor[j] = Vetor[j+1];
                    Vetor[j+1] = Temp;
                }
            }
        }

        int Step = 5;
        for(int i=0; i<Step; i++){  /// Clonando individuos
            for(int j=Step+i; j<POPULACAO; j=j+Step){
                for(int k=0; k<Vetor[j].TamanhoDNA; k++){
                    Vetor[j].DNA[k] = Vetor[i].DNA[k];        /// individuo 'j' recebe dna do individuo 'i'
                }
            }
        }

        for(int j=Step; j<POPULACAO; j++){
            int tipo;
            int mutations = rd.Next(1, (int)RangeRandom)+1;

            for(int k=0; k<mutations; k++){
                tipo = rd.Next(3);

                int indice = rd.Next(Vetor[j].TamanhoDNA);
                switch(tipo)
                {
                    case 0:
                    {
                        Vetor[j].DNA[indice] = Aleatorio.GetRandomValue();       /// Valor Aleatorio

                    }   break;
                    case 1:
                    {
                        double number = (rd.Next(10001) / 10000.0) + 0.5;
                        Vetor[j].DNA[indice] = Vetor[j].DNA[indice]*number;   /// Multiplica��o aleatoria

                    }   break;
                    case 2:
                    {
                        double number = Aleatorio.GetRandomValue()/100.0;
                        Vetor[j].DNA[indice] = Vetor[j].DNA[indice] + number; /// Soma aleatoria

                    }   break;
                }
            }
        }

        for(int j=0; j<POPULACAO; j++){  /// Copiando novos DNAs para DNAsDaVez
        
            for(int k=0; k<Individuos[j].TamanhoDNA; k++){
                DnaDaVez[j][k] = Individuos[j].DNA[k];
            }
        }

        Console.WriteLine("Range Random: "+ RangeRandom);
        RangeRandom = RangeRandom*0.99;
        if(RangeRandom < 5){
            RangeRandom = 5;
        }

    }

    static void VerificaFimDePartida(){
        if(mortos == POPULACAO || fimGer == true){
            RandomMutations();
            IniciarNovaPartida();
        }
    }

    static void IniciarNovaPartida(){
        mortos = 0;
        fimGer = false;

        for(int i=0; i<POPULACAO; i++){
            InicializarIndividuos(i, DnaDaVez[i],LARGURA_JANELA/2,ALTURA_JANELA/2);
        }
    }

    static void DesenharMapa(){

        for(int i=0;i<ALTURA_JANELA;i++){

            for(int j=0;j<LARGURA_JANELA;j++){
                if((i == 0) || (i == ALTURA_JANELA - 1)){
                    Console.Write("#");
                }else{
                    if((j==0) || (j==LARGURA_JANELA-1)){
                        Console.Write("#");
                    }else{
                        if((MelhorIndividuo.y == i) && (MelhorIndividuo.x==j)){
                            Console.Write("0");
                        } else if((Objetos[MelhorIndividuoI].y == i) && (Objetos[MelhorIndividuoI].x==j)){
                            Console.Write("x");
                        } else {
                            Console.Write(" ");
                        }

                        // for(int indice=0;indice<POPULACAO;indice++){
                        //     if((Individuos[indice].y == i) && (Individuos[indice].y==j)){
                        //         Console.Write("0");
                        //     } else if((Objetos[indice].y == i) && (Objetos[indice].x==j)){
                        //         Console.Write("x");
                        //     }
                        // } Serve para mostrar todos os individuos na tela
                    }
                    
                }
            }
            Console.WriteLine("");
        }
        Console.WriteLine("Individuo "+MelhorIndividuoI );
    }

    static void ControlarIndividuos(){
        double[] entrada = new double[NUMBER_OF_INPUT_NEURONS];
        double[] saida = new double[NUMBER_OF_OUTPUT_NEURONS];

        for(int i=0;i<POPULACAO;i++){
            if(Individuos[i].live){
                entrada[0] = Objetos[i].x - Individuos[i].x;
                entrada[1] = Objetos[i].y - Individuos[i].y;

                Individuos[i].rede.CopiarParaEntrada(entrada);
                Individuos[i].rede.CalcularSaida();
                Individuos[i].rede.CopiarDaSaida(saida);

                if(saida[0] > 0){
                    Individuos[i].x += SPEED;
                }
                if(saida[1] > 0){
                    Individuos[i].x -= SPEED;
                }
                if(saida[2] > 0){
                    Individuos[i].y += SPEED;
                }
                if(saida[3] > 0){
                    Individuos[i].y -= SPEED;
                }

                if(Individuos[i].y == Objetos[i].y && Individuos[i].x == Objetos[i].x){
                    Individuos[i].Fitness += 1;
                    Objetos[i].x = rd.Next(1,LARGURA_JANELA-1);
                    Objetos[i].y = rd.Next(1,ALTURA_JANELA-1);
                }

                // if(i == MelhorIndividuoI){
                //     if(saida[0] > 0){
                //     Console.WriteLine("Saida[0] x+");
                // }
                // if(saida[1] > 0){
                //     Console.WriteLine("Saida[1] x-");
                // }
                // if(saida[2] > 0){
                //     Console.WriteLine("Saida[2] y+");
                // }
                // if(saida[3] > 0){
                //     Console.WriteLine("Saida[3] y-");
                // }
                // }

                if(Individuos[i].x > LARGURA_JANELA){
                    Individuos[i].live = false;
                }
                if(Individuos[i].x < 0){
                    Individuos[i].live = false;
                }
                if(Individuos[i].y > ALTURA_JANELA){
                    Individuos[i].live = false;
                }
                if(Individuos[i].y < 0){
                    Individuos[i].live = false;
                }
            }
        }
    }

    static IA getMelhor(){
        double Maior = 0;
        int indice = 0;
        for(int i=0;i<POPULACAO;i++){
            if(Individuos[i].Fitness > Maior && Individuos[i].live){
                Maior = Individuos[i].Fitness;
                indice = i;
            }
        }
        MelhorIndividuoI = indice;
        return Individuos[indice];
    }

    static void UpdateMelhor(){
        
        MelhorIndividuo = getMelhor();
    }

    static void InicializarIndividuos(int indice,double[] DNA,int x,int y){
        Individuos[indice].x = x;
        Individuos[indice].y = y;
        Individuos[indice].Fitness = 0;
        Individuos[indice].live = true;

        for(int i=0; i<Individuos[indice].TamanhoDNA; i++){
            Individuos[indice].DNA[i] = DNA[i];
        }

        Individuos[indice].rede.CopiarVetorParaCamadas(Individuos[indice].DNA);
    }

    static void AlocarIndividuos(){
        int tamanho = 0;
        for(int i=0;i<POPULACAO;i++){
            Individuos[i].rede = new RedeNeural(NUMBER_OF_HIDDEN_LAYERS,NUMBER_OF_INPUT_NEURONS,NUMBER_OF_HIDDEN_NEURONS,NUMBER_OF_OUTPUT_NEURONS);
            tamanho = Individuos[i].rede.QuantidadePesos();
            Individuos[i].TamanhoDNA = tamanho;
            Individuos[i].DNA = new double[tamanho];
            DnaDaVez[i] = new double[tamanho];

            for(int j=0;j<tamanho;j++){
                DnaDaVez[i][j] = Aleatorio.GetRandomValue();
            }

            InicializarIndividuos(i,DnaDaVez[i],LARGURA_JANELA/2,ALTURA_JANELA/2);
        }
        MelhorIndividuo = getMelhor();
    }
}