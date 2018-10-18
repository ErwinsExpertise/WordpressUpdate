namespace System.Net
{
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;

    public class CookieAwareWebClient : WebClient
    {
        public void Login(string loginPageAddress, NameValueCollection loginData)
        {
            CookieContainer container;

            var request = (HttpWebRequest)WebRequest.Create(loginPageAddress);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0";
            request.Referer = "https://www.gplvault.com/my-account/";
            request.AllowAutoRedirect = false;
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";



            var query = string.Join("&",
              loginData.Cast<string>().Select(key => $"{key}={loginData[key]}"));

            var buffer = Encoding.ASCII.GetBytes(query);
            request.ContentLength = buffer.Length;
            var requestStream = request.GetRequestStream();
            requestStream.Write(buffer, 0, buffer.Length);
            requestStream.Close();

            container = request.CookieContainer = new CookieContainer();

            var response = request.GetResponse();
            Console.WriteLine(response.ToString());
            response.Close();
            CookieContainer = container;
        }

        public CookieAwareWebClient(CookieContainer container)
        {
            CookieContainer = container;
        }

        public CookieAwareWebClient()
          : this(new CookieContainer())
        { }

        public CookieContainer CookieContainer { get; private set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = CookieContainer;
            return request;
        }
    }
}