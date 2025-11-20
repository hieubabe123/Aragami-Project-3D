using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{

}

[System.Serializable]
public struct GameScene
{
    public static string Menu = "Menu";
    public static string Gameplay = "Gameplay";


}
