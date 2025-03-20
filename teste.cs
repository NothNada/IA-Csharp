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
    private static int LARGURA_JANELA = 10;
    private static int ALTURA_JANELA = 10;
    private static readonly int SPEED = 1;

    private static readonly int NUMBER_OF_HIDDEN_LAYERS = 1;
    private static readonly int NUMBER_OF_HIDDEN_NEURONS = 4;
    private static readonly int NUMBER_OF_INPUT_NEURONS = 2;
    private static readonly int NUMBER_OF_OUTPUT_NEURONS = 4;

    private static readonly int POPULACAO = 10;
    private static double[][] DnaDaVez = new double[POPULACAO][];
    private static IA[] Individuos = new IA[POPULACAO];
    private static IA MelhorIndividuo;
    private static int mortos = 0;

    private static bool fimGer = false;

    private static Obj[] Objetos = new Obj[POPULACAO];

    static void Main(string[] args){
        bool fim = false;

        ConsoleKeyInfo tecla;

        for(int i=0;i<POPULACAO;i++){
            Objetos[i] = new Obj(3,3);
        }

        InicializarIndividuos();

        while(!fim){



            DesenharMapa();

            tecla = Console.ReadKey();
            if(tecla.Key == ConsoleKey.Escape){
                fim = true;
            }
            if(tecla.KeyChar == 'c'){
                Console.Clear();
            }

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

    static void IniciarNovaPartida(){
        mortos = 0;
        fimGer = false;

        for(int i=0; i<POPULACAO_TAMANHO; i++){
            InicializarIndividuos(i, DNADaVez[i],LARGURA_JANELA/2,ALTURA_JANELA/2);
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
                        Console.Write(" ");
                    }

                    for(int indice=0;indice<POPULACAO;indice++){
                        if((Individuos[indice].y == i) && (Individuos[indice].x==j)){
                            Console.Write("0");
                        }

                        if((Objetos[indice].y == i) && (Objetos[indice].x==j)){
                            Console.Write("x");
                        }
                    }
                    
                }
            }
            Console.WriteLine("");
        }

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
            }
        }
    }

    static IA getMelhor(){
        double Maior = 0;
        int indice = 0;
        for(int i=0;i<POPULACAO;i++){
            if(Individuos[i].Fitness > Maior){
                Maior = Individuos[i].Fitness;
                indice = i;
            }
        }

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