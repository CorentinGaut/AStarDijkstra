using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    public string astarString = "AStar";
    public string dijkstrastring = "Dijkstra";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AStar()
    {
        SceneManager.LoadScene("Scenes/AStar");
    }

    public void Dijkstra()
    {
        SceneManager.LoadScene("Scenes/Dijkstra");
    }
}
