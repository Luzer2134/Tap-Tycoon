using UnityEngine;
using YG;
using TMPro;

public class GetAuthorize : MonoBehaviour
{
    [SerializeField] TMP_Text status;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        YG2.onGetSDKData += OnAuth;

        if (!YG2.player.auth)
        {
            YG2.OpenAuthDialog();
        }
        else
        {
            if (status)
            {
                status.text = YG2.player.name;
                status.text += '\n';
                status.text += YG2.player.id;           
            }
        }
    }

    private void OnAuth()
    {
        status.text = YG2.player.name;
        status.text += '\n';
        status.text += YG2.player.id;  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
