using Google.Apis.Auth.OAuth2; //google授權
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util.Store;  //用於儲存token
using MailKit.Net.Smtp;        //Mailkit SMTP寄信元件
using MailKit.Security;        //Mailkit安全性選項
using MimeKit;                 //建立郵件內容工具



namespace BoardGameFontier.Services
{
    public class GoogleMailService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _refreshToken;
        private readonly string _sender;

        //筆記IConfiguration = ASP.NET Core 的設定存取介面，用來安全地把敏感資訊
        //（像 Gmail 的 Client ID、Secret、Refresh Token）從 組態檔或環境變數拿進程式，而不用硬寫死在程式碼裡。
        public GoogleMailService(IConfiguration config) 
        {
            _clientId = config["GOOGLE_CLIENT_ID"] ?? "";
            _clientSecret = config["GOOGLE_CLIENT_SECRET"] ?? "";
            _refreshToken = config["GMAIL_REFRESH_TOKEN"] ?? "";
            _sender = config["GMAIL_SENDER"] ?? ""; // e.g. onlylovexiaohui@gmail.com
        }


        //此處做 GoogleAuthorizationCodeFlow筆記，
        public async Task SendMailAsync(string to, string subject, string body)
        {                                   // ↑收件人

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                    //↓全gmail權限
                }, //"https://mail.google.com/" //↓純最寄信的最小權限
                Scopes = ["https://www.googleapis.com/auth/gmail.send"] , //舊寫法new[] { "https://mail.google.com/" }
                // AccessType = "offline", // 要求可發 refresh token（第一次互動授權時用）
                Prompt = "consent"      //強制跳同意畫面以便拿 refresh token
            });

            // TokenResponse(容器)把「已存在的 refresh token」塞進去，建立憑證
            var token = new TokenResponse { RefreshToken = _refreshToken };
            var credential = new UserCredential(flow, "gmail-sender", token);
            //gmail-sender只是個標籤，寄信的帳號存在於token裡的RefreshToken，裡面包著我的email帳號

            //  以 refresh token 直接換新的 access token（無頭）
            //  GetAccessTokenForRequestAsync 會自動 refresh；保險起見先呼叫一次 Refresh
            await credential.RefreshTokenAsync(CancellationToken.None);
            var accessToken = await credential.GetAccessTokenForRequestAsync();


            //  建立信件
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BoardGameFontier", "onlylovexiaohui@gmail.com"));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject; //信件主旨(理論上應該就寫驗證信)
            message.Body = new TextPart("html") { Text = body };//這邊body是信件內容
                           
            // 用 MailKit + OAuth2 寄出
            using (var client = new SmtpClient()) // SmtpClient是MailKit提供的
            { // smtp.gmail.com是Gmail的SMTP伺服器  587是StartTLS的標準埠號  StartTls 是一種加密方式，會先建立不加密的連線，再升級為加密連線（比 SSL 更安全且常用）
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                //Authenticate是執行登入/認證的方法  ↓用 OAuth2 token 做為身分認證
                await client.AuthenticateAsync(new SaslMechanismOAuth2( _sender, accessToken));
                await client.SendAsync(message);
                await client.DisconnectAsync(true); //中斷連線釋放資源用
            }
        }

        public string RegisterMailBody(string verifyUrl)
        {
            //這邊產生驗證的連結    
            //自我備註一下使用 $@ 是 C# 的 多行字串插值語法
            //@ 允許多行，$ 表示可以用 {} 插入變數（這邊插入了 verifyUrl）。
            //未來若還有時間改成比較華麗的html信件這邊可能會要改掉，再說
            return $@"
                <p>您好新玩家！</p>
                <p>請點擊下方連結以完成註冊：</p>
                <p><a href='{verifyUrl}'>點我完成註冊</a></p>
                <p>本連結將於 5 分鐘後失效。</p>";
        }
    }
}
