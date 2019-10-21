using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class AStar : MonoBehaviour
{

    public int xTaille;
    public int yTaille;
    public float frameDeplacement = 2f;
    Noeud[,] graphe;
    Noeud NoeudDepart;
    Noeud NoeudArrive;
    public Tilemap map;
    public Tile ground, wall, visite, chemin , player, Down, Up, Right, point;
    Vector3Int posPlayer;
    int randomX;
    int randomY;
    public int nbPoints = 0;
    public Text text;

    void Start()
    {
        graphe = new Noeud[xTaille, yTaille];
        NoeudDepart = new Noeud(10, 10, 0, null,false);

        createMap();
        map.SetTile(new Vector3Int(NoeudDepart.x, NoeudDepart.y, 1), player);
        posPlayer = new Vector3Int(NoeudDepart.x, NoeudDepart.y, 1);

        createGraphe();
        generatePoint();
        NoeudArrive = new Noeud(randomX, randomY, 0, null, false);
        Noeud arrive = AStarAlgorithme(NoeudDepart, NoeudArrive);
        //DessinChemin(arrive);
    }

    // Update is called once per frame
    void Update()
    {
        CheckKeys();
        Noeud player = new Noeud(posPlayer.x,posPlayer.y, 0, null, false);
        createMap();

        if (posPlayer.x == randomX && posPlayer.y == randomY)
        {
            nbPoints++;
            text.text = nbPoints + " : Points";
            generatePoint();
            NoeudArrive = new Noeud(randomX, randomY, 0, null, false);
        }
        Noeud arrive = AStarAlgorithme(player, NoeudArrive);

        DessinChemin(arrive);

    }

    void createMap()
    {
        for (int j = 0; j < yTaille; j++)
        {
            for (int i = 0; i < xTaille; i++)
            {
                Tile tile = ground;
                if(i == 0 || i == xTaille-1 || j == 0 || j == yTaille - 1 || (i < 10 && j == 35) || (i < 10 && j == 36) || (i < 10 && j == 15) || (i < 10 && j == 16))
                {
                    tile = wall;
                }
                if((i > xTaille - 11 && j == 35) || (i > xTaille - 11 && j == 36) || (i > xTaille - 11 && j == 15) || (i > xTaille - 11 && j == 16))
                {
                    tile = wall;
                }

                if ((i == 25 && j > 35) || (i == 26 && j > 35) || (i == 25 && j < 15) || (i == 26 && j < 15))
                {
                    tile = wall;
                }

                if ((i > 15 && j == 25) && (i < 35 && j == 25) || (i > 15 && j == 26) && (i < 35 && j == 26) || (i > 15 && j == 24) && (i < 35 && j == 24))
                {
                    tile = wall;
                }

                map.SetTile(new Vector3Int(i, j, 0), tile);
            }
        }
    }


    void createGraphe()
    {
        for (int j = 0; j < yTaille; j++)
        {
            for (int i = 0; i < xTaille; i++)
            {
                bool obstacle = false;
                if (i == 0 || i == xTaille - 1 || j == 0 || j == yTaille - 1 || (i < 10 && j == 35) || (i < 10 && j == 36) || (i < 10 && j == 15) || (i < 10 && j == 16))
                {
                    obstacle = true;
                }
                if ((i > xTaille - 11 && j == 35) || (i > xTaille - 11 && j == 36) || (i > xTaille - 11 && j == 15) || (i > xTaille - 11 && j == 16))
                {
                    obstacle = true;
                }

                if ((i == 25 && j > 35) || (i == 26 && j > 35) || (i == 25 && j < 15) || (i == 26 && j < 15))
                {
                    obstacle = true;
                }

                if ((i > 15 && j == 25) && (i < 35 && j == 25) || (i > 15 && j == 26) && (i < 35 && j == 26) || (i > 15 && j == 24) && (i < 35 && j == 24))
                {
                    obstacle = true;
                }

                Noeud n = new Noeud(i, j, int.MaxValue, null, obstacle);
                graphe[i, j] = n;
            }
        }
    }
    Noeud AStarAlgorithme(Noeud noeudDepart, Noeud noeudArrive)
    {
        //--------------initialisation--------------
        List<Noeud> Testes = new List<Noeud>();
        List<Noeud> noeudAtester = new List<Noeud>();

        noeudAtester.Add(noeudDepart);
        //--------------depart de l'algo--------------
        while (noeudAtester.Count > 0)
        {
            //--------------cout minimum de la list--------------
            Noeud s1 = choixDistancePlusPetite(noeudAtester);
            noeudAtester.Remove(s1);
            
            Testes.Add(s1);

            //--------------list des voisins de s1--------------
            List<Noeud> voisinsS1 = voisins(s1, Testes);

            //--------------test si neoud arrive--------------
            if (s1.x == noeudArrive.x && s1.y == noeudArrive.y)
            {
                return s1;
            }

            //--------------mise a jour de la distance--------------
            foreach (Noeud s2 in voisinsS1)
            {
                if (!noeudAtester.Contains(s2) && !Testes.Contains(s2))
                {
                    int heuristique = Mathf.Max(Mathf.Abs(s2.x - noeudArrive.x), Mathf.Abs(s2.y - noeudArrive.y));
                    s2.distance = s1.distance + 1;
                    s2.heuristique = heuristique;
                    map.SetTile(new Vector3Int(s2.x, s2.y, 0), visite);
                    noeudAtester.Add(s2);
                    graphe[s2.x, s2.y].parent = s1;
                }
            }
        }
        return null;
    }


    Noeud choixDistancePlusPetite(List<Noeud> noeudAtester)
    {
        int mini = int.MaxValue;

        Noeud sommet = new Noeud(0, 0, -1, null , false);
        foreach (Noeud n in noeudAtester)
        {
            if (n.heuristique < mini)
            {
                mini = n.heuristique;
                sommet = n;

            }
        }
        return sommet;
    }

    List<Noeud> voisins(Noeud s1, List<Noeud> Tester)
    {
        List<Noeud> voisins = new List<Noeud>();
        // x - 1 -------------------------------------
        if (s1.x - 1 >= 0 && graphe[s1.x - 1, s1.y].obstacle == false)
        {
            voisins.Add(graphe[s1.x - 1, s1.y]);
        }
        // x + 1 -------------------------------------
        if (s1.x + 1 < xTaille && graphe[s1.x + 1, s1.y].obstacle == false)
        {
            voisins.Add(graphe[s1.x + 1, s1.y]);
        }
        // y - 1 -------------------------------------
        if (s1.y - 1 >= 0 && graphe[s1.x, s1.y - 1].obstacle == false)
        {
            voisins.Add(graphe[s1.x, s1.y - 1]);
        }
        // y + 1 -------------------------------------
        if (s1.y + 1 < yTaille && graphe[s1.x, s1.y + 1].obstacle == false)
        {
            voisins.Add(graphe[s1.x, s1.y + 1]);
        }
        // x + 1 : y + 1 -------------------------------------
        if (s1.y + 1 < yTaille && s1.x + 1 < xTaille && graphe[s1.x + 1, s1.y + 1].obstacle == false)
        {
            voisins.Add(graphe[s1.x + 1, s1.y + 1]);
        }
        // x - 1 : y + 1 -------------------------------------
        if (s1.y + 1 < yTaille && s1.x - 1 >= 0 && graphe[s1.x - 1, s1.y + 1].obstacle == false)
        {
            voisins.Add(graphe[s1.x - 1, s1.y + 1]);
        }
        // x - 1 : y - 1 -------------------------------------
        if (s1.y - 1 > 0 && s1.x - 1 > 0 && graphe[s1.x - 1, s1.y - 1].obstacle == false)
        {
            voisins.Add(graphe[s1.x - 1, s1.y - 1]);
        }
        // x + 1 : y - 1 -------------------------------------
        if (s1.y - 1 > 0 && s1.x + 1 < xTaille && graphe[s1.x + 1, s1.y - 1].obstacle == false)
        {
            voisins.Add(graphe[s1.x + 1, s1.y - 1]);
        }
        return voisins;
    }

    void DessinChemin(Noeud arrive)
    {
        float index = 0;
        while (arrive.parent != null)
        {
            if (index >= frameDeplacement)
            {
                
                map.SetTile(new Vector3Int(arrive.x, arrive.y, 0), chemin);
                arrive = arrive.parent;
                index = 0;
            }
            index +=  Time.deltaTime;
        }
    }

    void CheckKeys()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (graphe[posPlayer.x - 1, posPlayer.y].obstacle == false)
            {
                map.SetTile(posPlayer, ground);
                posPlayer = Vector3Int.left + posPlayer;
                map.SetTile(posPlayer, player);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (graphe[posPlayer.x + 1, posPlayer.y].obstacle == false)
            {
                map.SetTile(posPlayer, ground);
                posPlayer = Vector3Int.right + posPlayer;
                map.SetTile(posPlayer, Right);
            }
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (graphe[posPlayer.x, posPlayer.y + 1].obstacle == false)
            {
                map.SetTile(posPlayer, ground);
                posPlayer = Vector3Int.up + posPlayer;
                map.SetTile(posPlayer, Up);
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (graphe[posPlayer.x,posPlayer.y - 1].obstacle == false)
            {
                map.SetTile(posPlayer, ground);
                posPlayer = Vector3Int.down + posPlayer;
                map.SetTile(posPlayer, Down);
            }
        }
    }

    void generatePoint()
    {
        randomX = Random.Range(1,xTaille - 2);
        randomY = Random.Range(1, yTaille - 2);

        do{
            randomX = Random.Range(1, xTaille - 2);
            randomY = Random.Range(1, yTaille - 2);

        } while (graphe[randomX, randomY].obstacle == true);

        map.SetTile(new Vector3Int(randomX, randomY, 1), point);
    }

    void phantomMovment()
    {
        //Time.deltaTime
    }
}
