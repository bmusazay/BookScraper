using System;

using System.Web;
using System.IO;
using System.Net;
using System.Text;

public class HttpSession
{

    public CookieContainer cookieJar;
    public string contentType;
    public string userAgent;
    public string referer;
    public string host;
    public string proxy;
    public string html;
    

	public HttpSession()
	{
        cookieJar = new CookieContainer();
        proxy = "";
        contentType = "";
        userAgent = "";
        referer = "";
        host = "";
        html = "";
	}

    public string getPage(string getUrl)
    {     
        HttpWebRequest getPage = (HttpWebRequest)WebRequest.Create(getUrl);
        getPage.Method = "GET";
        getPage.UserAgent = userAgent;
        getPage.CookieContainer = cookieJar;
        getPage.Timeout = 30000;
        if (proxy != "") getPage.Proxy = createProxy(proxy);
        if (host != "") getPage.Host = host;
        HttpWebResponse getPageResponse = (HttpWebResponse)getPage.GetResponse();
        Stream datastream = getPageResponse.GetResponseStream();
        StreamReader reader = new StreamReader(datastream);
        string response = reader.ReadToEnd();
        cookieJar = getPage.CookieContainer;
        reader.Close();
        datastream.Close();
        getPageResponse.Close();
        html = response;
        return response;
    }

    public string postPage(string postUrl, string postData)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        HttpWebRequest submitRequest = (HttpWebRequest)WebRequest.Create(postUrl);
        submitRequest.Method = "POST";
        submitRequest.ContentLength = byteArray.Length;
        submitRequest.CookieContainer = cookieJar;
        if (contentType != "")
        {
            submitRequest.ContentType = contentType;
        }
        else
        {
            submitRequest.ContentType = "application/x-www-form-urlencoded";
        }
        if (userAgent != "") submitRequest.UserAgent = userAgent;
        if (referer != "") submitRequest.Referer = referer;
        if (proxy != "") submitRequest.Proxy = createProxy(proxy);
        if (host != "") submitRequest.Host = host;
        submitRequest.Timeout = 45000;
        Stream datastream = submitRequest.GetRequestStream();
        datastream.Write(byteArray, 0, byteArray.Length);
        datastream.Close();

        HttpWebResponse submitResponse = (HttpWebResponse)submitRequest.GetResponse();
        datastream = submitResponse.GetResponseStream();
        StreamReader reader = new StreamReader(datastream);
        string response = reader.ReadToEnd();
        cookieJar = submitRequest.CookieContainer;
        reader.Close();
        datastream.Close();
        submitResponse.Close();
        html = response;
        return response;
    }

    public string extractFromHtml(string beginningString, string endString, int offset)
    {
        string parse;
        int start = html.IndexOf(beginningString) + offset;
        int end = html.IndexOf(endString);
        parse = html.Substring(start, end - start);
        return parse;
    }

    private WebProxy createProxy(string ipPort)
    {
        string[] proxyport = ipPort.Split(':');
        string ip = proxyport[0];
        string port = proxyport[1];
        WebProxy proxyObject = new WebProxy(ip, Convert.ToInt32(port));
        return proxyObject;
    }

    public void setUserAgent()
    {
        string[] agentList = {"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)", 
                               "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:29.0) Gecko/20120101 Firefox/29.0",
                               "Mozilla/5.0 (Windows; U; MSIE 9.0; WIndows NT 9.0; en-US))",
                               "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; Media Center PC 6.0; InfoPath.3; MS-RTC LM 8; Zune 4.7)",
                               "Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2049.0 Safari/537.36",
                               "Mozilla/5.0 (Windows NT 5.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1985.67 Safari/537.36",
                               "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/36.0.1944.0 Safari/537.36",
                               "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_9_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/32.0.1664.3 Safari/537.36",
                               "Mozilla/5.0 (Windows NT 6.1; Win64; x64; rv:25.0) Gecko/20100101 Firefox/29.0",
                               "Mozilla/5.0 (X11; OpenBSD amd64; rv:28.0) Gecko/20100101 Firefox/28.0"};
        Random random = new Random();
        userAgent = agentList[random.Next(0,9)];
    }

}
