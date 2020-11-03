using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    #region Variables
    public byte[] continueGameCounter;
    [SerializeField] private List<GameObject> _players = null;
    public static GameController instance;
    
    #endregion

    #region #Properties

    public List<GameObject> Players => _players;
    
    #endregion

    void Start()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void RemovePlayerFromScene(Penosa player)
    {
        player.gameObject.SetActive(false);
        continueGameCounter[player.id]--;
        if(continueGameCounter[player.id] == 0) 
        {
            // Game over...
        }
        else
        {   
            // Call Respawn Countdown  
        }
    }

    void DontDestroyOnLoad()
    {

    }
}