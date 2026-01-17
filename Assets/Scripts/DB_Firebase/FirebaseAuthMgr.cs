using Firebase.Extensions;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Database;
using TMPro;

public class FirebaseAuthMgr : MonoBehaviour
{
    public FirebaseAuth auth; //인증 진행을 위한 객체
    public static FirebaseUser user; //인증되고나서, 인증된 유저 정보를 가지고있는다
    public static DatabaseReference dbRef; //DB에 대한 정보를 여러 씬이서 다양하게 쓰려고

    [SerializeField] Button _startButton; //게임시작버튼(로그인안되면 비활성화)
    [SerializeField] Button _loginButton; //로그인 버튼
    [SerializeField] Button _registerButton; //회원가입 버튼
    [SerializeField] TMP_InputField _emailField; //이메일입력란
    [SerializeField] TMP_InputField _pwField; //패스워드 입력란
    [SerializeField] TMP_InputField _nickField; //닉네임 입력란

    [SerializeField] TextMeshProUGUI _warningText; //로그인 실패시 띄울 텍스트
    [SerializeField] TextMeshProUGUI _confirmText; //로그인 성공시 띄울 텍스트


    private void Awake()
    {
        //프로그램 구동과 동시에 비동기로 의존성을 체크한다((SDK, 네이티브 라이브러리, 설정)등 )
        //PC가 아닌 모바일 같은것에서 ContinueWith 코드 수행하면 안될수있음, 그래서 ContinueWithOnMainThread 사용
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == Firebase.DependencyStatus.Available) //가능하다면
            {
                auth = Firebase.Auth.FirebaseAuth.DefaultInstance; //인증정보 기억
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format("뭔가 잘못됨" + task.Result));
            }
        });
        //버튼 등록
        SetButtonListener();
    }
    private void Start()
    {
        _startButton.interactable = false; //초기화 성공 후 인게임 버튼 활성화
        _warningText.text = "";
        _confirmText.text = "";
    }
    private void OnDestroy()
    {
        RemoveButtonListener();
    }
    private void SetButtonListener()
    {
        _loginButton.onClick.AddListener(Login);
        _registerButton.onClick.AddListener(Register);
    }
    private void RemoveButtonListener()
    {
        _loginButton.onClick.RemoveAllListeners();
        _registerButton.onClick.RemoveAllListeners();
    }
    public void Login() //로그인 버튼에 연동되어있음. 클릭하면 동작할 메서드
    {
        StartCoroutine(LoginCor(_emailField.text, _pwField.text));
    }

    IEnumerator LoginCor(string email, string password)
    {
        Task<AuthResult> LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => LoginTask.IsCompleted);
        //로그인 시도가 완료되었다면 진행
        if (LoginTask.Exception != null) //로그인 문제가 생겼다면
        {
            Debug.Log("다음과 같은 이유로 로그인 실패: " + LoginTask.Exception);
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
            string message = "";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;
                case AuthError.WrongPassword:
                    message = "패스워드 틀림";
                    break;
                case AuthError.InvalidEmail:
                    message = "이메일 형식이 옳지 않음";
                    break;
                case AuthError.UserNotFound:
                    message = "아이디가 존재하지 않음";
                    break;
                default:
                    message = "관리자에게 문의 바랍니다";
                    break;
            }
            _warningText.text = message;
        }
        else //로그인 성공했다면
        {
            user = LoginTask.Result.User;
            _warningText.text = "";
            _nickField.text = user.DisplayName;
            _confirmText.text = "로그인 완료, 반갑습니다" + user.DisplayName + "님";
            _startButton.interactable = true;
        }
    }

    IEnumerator RegisterCor(string email, string password, string userName)
    {
        Task<AuthResult> RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => RegisterTask.IsCompleted);
        if (RegisterTask.Exception != null)
        {
            Debug.LogWarning(message: "실패 사유" + RegisterTask.Exception);
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "회원가입 실패";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "이메일 누락";
                    break;
                case AuthError.MissingPassword:
                    message = "패스워드 누락";
                    break;
                case AuthError.WeakPassword:
                    message = "패스워드 약함";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "중복 이메일";
                    break;
                default:
                    message = "기타 사유. 관리자 문의 바람";
                    break;
            }
            _warningText.text = message;
        }
        else //생성완료
        {
            user = RegisterTask.Result.User;
            if (user != null)
            {
                //로컬에서 닉네임을 기입후 서버에 업데이트해주기위해서 사용
                UserProfile profile = new UserProfile { DisplayName = userName };
                //파이어베이스에 올림
                Task profileTask = user.UpdateUserProfileAsync(profile);
                yield return new WaitUntil(() => profileTask.IsCompleted);

                if (profileTask.Exception != null) //문제가 생겼다면
                {
                    Debug.LogWarning("닉네임설정 실패" + profileTask.Exception);
                    FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                    AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                    _warningText.text = "닉네임 설정 실패하였습니다";
                }
                else //닉네임 설정 성공
                {
                    _warningText.text = "";
                    _confirmText.text = "생성완료, 반갑습니다 " + user.DisplayName + "님";
                    //_startButton.interactable = true;
                }
            }
            //닉네임에 욕, [GM] 이런 표시를 막기위해서 Regex라는 것을 써서 막을 수도 있음


        }
    }

    public void Register() //회원가입 버튼에 연동되어있음.
    {
        StartCoroutine(RegisterCor(_emailField.text, _pwField.text, _nickField.text));
    }
}
