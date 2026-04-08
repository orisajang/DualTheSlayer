using Firebase.Auth;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PhotonChatScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField messageInput;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private Button sendMessageButton;
    StringBuilder sb = new StringBuilder();

    private void Awake()
    {
        sendMessageButton.onClick.AddListener(SendMessage);
    }
    private void OnDestroy()
    {
        sendMessageButton.onClick.RemoveAllListeners();
    }
    public void SendMessage()
    {
        //버튼클릭했을때 메세지를 보내자
        //RPC로 보낸다고함.
        photonView.RPC(nameof(AddChatLog), RpcTarget.All, FirebaseAuth.DefaultInstance.CurrentUser.UserId, messageInput.text);
        messageInput.text = "";
    }

    [PunRPC]
    private void AddChatLog(string id, string text)
    {
        sb.AppendLine($"{id}: {text}");
        chatText.text = sb.ToString();
    }

}
