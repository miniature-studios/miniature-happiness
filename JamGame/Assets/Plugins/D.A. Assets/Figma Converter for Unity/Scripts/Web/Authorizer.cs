using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DA_Assets.FCU
{
    [Serializable]
    public class Authorizer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator Auth()
        {
            DAResult<AuthResult> authResult = default;

            yield return StartAuthThread(x => authResult = x);

            if (authResult.Success)
            {
                monoBeh.FigmaSession.AddNew(authResult.Object.AccessToken);
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_cant_auth.Localize(authResult.Error.Message, authResult.Error.Status));
            }
        }

        private IEnumerator StartAuthThread(Return<AuthResult> @return)
        {
            string code = "";

            bool gettingCode = true;

            Thread thread = null;

            DALogger.Log(FcuLocKey.log_open_auth_page.Localize());

            thread = new Thread(x =>
            {
                Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 1923);

                server.Bind(endpoint);
                server.Listen(1);

                Socket socket = server.Accept();

                byte[] bytes = new byte[1000];
                socket.Receive(bytes);
                string rawCode = Encoding.UTF8.GetString(bytes);

                string toSend = "HTTP/1.1 200 OK\nContent-Type: text/html\nConnection: close\n\n" + @"
                    <html>
                        <head>
                            <style type='text/css'>body,html{background-color: #000000;color: #fff;font-family: Segoe UI;text-align: center;}h2{left: 0; position: absolute; top: calc(50% - 25px); width: 100%;}</style>
                            <title>Wait for redirect...</title>
                            <script type='text/javascript'> window.onload=function(){window.location.href='https://figma.com';}</script>
                        </head>
                        <body>
                            <h2>Authorization completed. The page will close automatically.</h2>
                        </body>
                    </html>";

                bytes = Encoding.UTF8.GetBytes(toSend);

                NetworkStream stream = new NetworkStream(socket);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();

                stream.Close();
                socket.Close();
                server.Close();

                code = rawCode.GetBetween("?code=", "&state=");
                gettingCode = false;
                thread.Abort();
            });

            thread.Start();

            int state = Random.Range(0, int.MaxValue);
            string formattedOauthUrl = string.Format(FcuConfig.OAuthUrl, FcuConfig.ClientId, FcuConfig.RedirectUri, state.ToString());

            Application.OpenURL(formattedOauthUrl);

            while (gettingCode)
            {
                yield return WaitFor.Delay01();
            }

            DARequest tokenRequest = RequestCreator.CreateTokenRequest(code);

            yield return monoBeh.RequestSender.SendRequest(tokenRequest, @return);
        }
    }
}