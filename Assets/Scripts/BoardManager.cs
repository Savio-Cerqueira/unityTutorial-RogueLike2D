using System.Collections.Generic;       //Permite a utilização de "List"
using System;
using UnityEngine;
using Random = UnityEngine.Random;      //Força Random a usar o gerador aleatório da Unity

public class BoardManager : MonoBehaviour 
{
	//Usando "Serializable" é possível embutir uma classe com subpropriedades no Inspector
	[Serializable]
	public class Count
	{
		public int minimum;                  //Valor mínimo para classe Count
		public int maximum;                  //Valor máximo para classe Count
		//Construtor da tarefa
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
	//Variáveis Públicas
	public int columns = 8;                                       //Número de colunas no tabuleiro
	public int rows = 8;                                          //Número de linhas no tabuleiro
	public Count wallCount = new Count (5,9);                     //Limite inferior e superior para o número aleatório de Muros por nível
	public Count foodCount = new Count (1,5);                     //Limite inferior e superior para o número aleatório de itens por nível                      
	public GameObject exit;                                       //Prefab do tile exit
	public GameObject[] floorTiles;                               //Array dos prefabs floor
	public GameObject[] wallTiles;                                //Array dos prefabs wall
	public GameObject[] foodTiles;                                //Array dos prefabs food
	public GameObject[] enemyTiles;                               //Array dos prefabs enemy
	public GameObject[] outerWallTiles;                           //Array dos prefabs outerwall
	//Variáveis Privadas
	private Transform boardHolder;                                //Variável que armazena uma referência pra transformação do tabuleiro
	private List <Vector3> gridPositions = new List<Vector3>();   //Lista de possíveis locais para armazenar as tiles
	//Limpa a lista gridPositions e prepara a geração de um novo tabuleiro
	void InitialiseList()
	{
		//Limpa a lista
		gridPositions.Clear();
		//Loop no eixo x(colunas)
		for (int x = 1; x < columns - 1; x++)
		{
			//Em cada coluna, loop no eixo y(linhas)
			for (int y = 1; y < rows - 1; y++)
			{
				//Em cada índice adiciona um novo objeto Vector3
				gridPositions.Add(new Vector3(x,y,0f));
			}
		}
	}
	//Configura os muros exteriores e o chão(plano de fundo) do tabuleiro;
	void BoardSetup()
	{
		//Instancia Tabuleiro e configura boardHolder pra transform
		boardHolder = new GameObject("Board").transform;
        //Loop no eixo x, começando de -1 (Para preencher as esquinas) com tiles floor ou outerwall
		for(int x = -1;x < columns + 1;x++)
		{
			//Loop no eixo y, começando de -1 para colocar um tile floor ou outerwall
			for(int y = -1; y < rows + 1; y++)
			{
				//Escolhe um tile aleatório do array de tiles floor e prepara para instanciar
				GameObject toInstantiate = floorTiles[Random.Range (0, floorTiles.Length)];
				//Checa se a posição atual está num limite do tabuleiro, e se está, escolhe um prefab aleatório do array de tiles outerwall
				if(x == -1||x == columns|| y == -1|| y == rows)
				{
					toInstantiate = outerWallTiles[Random.Range (0, outerWallTiles.Length)];
				}
				//Instancia a instância GameObject usando o prefab escolhido para toInstaintiate no Vector3 correspondendo pra atual posição no loop, lançar para GameObject
				GameObject instance = Instantiate(toInstantiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;
				//Configurar o pai do novo objeto instanciadocomo boardHolder, apenas organizacional para evitar desordem na hierarquia
				instance.transform.SetParent(boardHolder);
			}
		}
	}
	//RandomPos retorna uma posição aleatória da lista gridPositions
	Vector3 RandomPos()
	{
		//Declara um randomIndex inteiro, configura o valor pra um número aleatório entre 0 e o total de itens na List gridPositions 
		int randomIndex = Random.Range(0, gridPositions.Count);
		//Declara uma variável do tipo Vector3 chamada randomPosition, configura o valor para entrada de randomIndex na List gridPositions
		Vector3 randomPos = gridPositions[randomIndex];
		//Remove a entrada em randomIndex da lista, assim não pode ser reutilizada
		gridPositions.RemoveAt(randomIndex);
        //Retorna a posição Vector3 selecionada aleatoriamente
		return randomPos;
	}
	//LayoutObjectAtRandom recebe um array de game objects para escolher entre um mínimo e um máximo para o número de objetos a criar. 
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
	{
		//Escolhe um número aleatório de objetos a instanciar entre os limites mínimo e máximo
		int objectCount = Random.Range(minimum, maximum + 1);
		//Instancia objetos até o o limite aleatório ser alcançado
		for(int i = 0; i < objectCount; i++)
		{
			//Escolhe uma posição para randomPos escolhendo um posição aleatória da lista de Vector3 disponíveis armazenadas em gridPositions  
			Vector3 randomPos = RandomPos();
			//Escolhe um tile aleatório e o atribui para tileChoice
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			//Instancia tileChoice na posição retornada por RandomPos sem mudanças na rotação
			Instantiate (tileChoice, randomPos, Quaternion.identity);
		}
	}
	//SetupScene inicializa o nível e chama as funções prévias para formar o tabuleiro
	public void SetupScene(int level)
	{
		//Cria os outerwalls e floor
		BoardSetup();
		//Reseta a lsita de gridPositions
		InitialiseList();
		//Instancia um número aleatório de tiles wall baseado nos valores minimo e máximo, em posições aleatórias
		LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
		//Instancia um número aleatório de tiles food baseado nos valores minimo e máximo, em posições aleatórias
		LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
		//Determina o número de inimigos no nível atual, baseado numa progressão logarítmica
		int enemyCount = (int)Mathf.Log(level,2f);
		//Instancia um número aleatório de inimigos baseado nos valores mínimo e máximo, em poições aleatórias
		LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
		//Instancia o tile exit na esquina superior direita do tabuleiro
		Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
	}
}
