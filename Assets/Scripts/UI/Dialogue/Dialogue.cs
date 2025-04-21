using UnityEngine;

[System.Serializable]       //직렬화를 통해서 인스펙터 창에서 데이터 건드릴 수 있게 됨

public class Dialogue 
{
    [Tooltip("대사 치는 캐릭터 이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] contexts;

}

[System.Serializable]
public class DialogueEvent
{
    public string name;            //어디서의 대사인지 알기위한 이름

    public Vector2 line;           //대사 추출용 
    public Dialogue[] dialogues;   
}