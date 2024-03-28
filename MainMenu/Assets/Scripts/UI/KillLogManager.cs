using UnityEngine;
using UnityEngine.UI;

public class KillLogManager : MonoBehaviour
{
    public static KillLogManager Instance;

    [SerializeField] private GameObject killLogPrefab;
    [SerializeField] private Transform logContainer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("살았다?");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("죽는다 ..");

        }
    }

    public void CreateKillLog(string killer, string victim)
    {
        Debug.Log("실행은 하니? ..");

        GameObject log = Instantiate(killLogPrefab, logContainer);
        Text[] texts = log.GetComponentsInChildren<Text>();
        if (texts.Length >= 2)
        {
            texts[0].text = killer; // 첫 번째 Text 컴포넌트에 killer 이름 설정
            texts[1].text = victim; // 두 번째 Text 컴포넌트에 victim 이름 설정
        }
        Destroy(log,5f);
        // 예: log.GetComponent<KillLogUI>().Setup(killer, victim);
        // Setup 메소드는 KillLogUI 컴포넌트에서 킬러와 피해자의 이름으로 UI를 설정합니다.
    }
}