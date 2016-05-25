using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafenetIntegration.Safenet.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SafenetIntegration.Safenet
{
  public class SafenetUtilities
  {
    public const string DEMOMA = "1";
    public const string MXBWI = "2";
    public const string PRODUCT_KEY = "PRODUCT_KEY";
    public const string PROTECTIONKEY_UPDATE = "PROTECTIONKEY_UPDATE";
    public const string HARDWARE_KEY = "HARDWARE_KEY";
    public const string ENFORCEMENT_SENTINEL_LDK = "1";
    public const string ENFORCEMENT_SENTINEL_CLOUD = "2";
    public const string DEFAULT_DEMOMA_USERNAME = "testservicedemoma";
    public const string DEFAULT_DEMOMA_PASSWORD = "Caram3ll3!";
    public const string DEFAULT_DEMOMA_URI = "https://licensing.dptcorporate.com";
    public const string DPT_PRODUCT_LIST = "DptProductList.txt";
    public const string SERVER_MAP_PATH = "~/App_Data/";
    public const string CONCURRENT_ISTANCES = "CONCURRENT_INSTANCES";
    public const string CONCURRENT_INSTANCES_UNLIMITED = "UNLIMITED";
    public Dictionary<string, string> PRODUCT_LIST_DPTASSOCIATION { get; set; }
    private static readonly Lazy<SafenetUtilities> _instance
    = new Lazy<SafenetUtilities>(() => new SafenetUtilities());

    public static SafenetUtilities Instance
    {
      get
      {
        return _instance.Value;
      }
    }

    public SafenetUtilities()
    {
      this.ReadDptProductDictionary();
    }

    public void ReadDptProductDictionary()
    {
      string root = HttpContext.Current.Server.MapPath(SERVER_MAP_PATH);
      string file = Path.Combine(root, DPT_PRODUCT_LIST);
      String content = "";
      try
      {
        using (StreamReader sr = new StreamReader(file))
        {
          content = sr.ReadToEnd();
        }
      }
      catch (System.Exception e)
      {
        throw new System.Exception(file, e);
      }
      string[] splitted = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
      PRODUCT_LIST_DPTASSOCIATION = new Dictionary<string, string>();
      for (int i = 0; i < splitted.Length; i++)
      {
        PRODUCT_LIST_DPTASSOCIATION.Add(splitted[i].Replace(".", string.Empty).ToLower(), splitted[i]);
      }
    }

    public static string GetLocale(string locale)
    {
      switch (locale.ToLower())
      {
        case "english":
          return "EN";
        case "italian":
          return "IT";
        case "german":
          return "DE";
        case "japanese":
          return "JP";
        case "french":
          return "FR";
        default:
          return "EN";
      }
    }

    public static JToken GetHeaders(WebHeaderCollection headers)
    {
      JObject jHeader = new JObject();
      for (int i = 0; i < headers.Count; ++i)
      {
        string header = headers.GetKey(i);
        string value = JsonConvert.SerializeObject(headers.GetValues(i));
        jHeader.Add(header, JArray.Parse(value));

      }
      return (JToken)jHeader;
      //var json = JsonConvert.SerializeObject(jHeader);
      //return json;
    }

    public static void ValidateParameters(JObject safenetParams)
    {
      JToken crmId;
      JToken entType;
      JToken productName;
      if (!safenetParams.TryGetValue("CrmId", out crmId) ||
        !safenetParams.TryGetValue("EntType", out entType) ||
        !safenetParams.TryGetValue("ProductName", out productName) ||
        string.IsNullOrEmpty(crmId.Value<string>()) ||
        string.IsNullOrEmpty(entType.Value<string>()) ||
        productName.Value<JArray>().Count == 0
        )
        throw new SafenetException("Missing some Parameters in the request", HttpStatusCode.BadRequest);
    }

    public static string FormatConcurrentIstances(int cInst)
    {
      return cInst >= 10000000 ? "10000000" : cInst.ToString();
    }
    public void UpdateProducts(JObject jObject)
    {
      string root = HttpContext.Current.Server.MapPath("~/App_Data/");
      string file = Path.Combine(root, "ProductList.txt");  // ### TODO: get the tenant file
      JArray productList = jObject["listResponse"]["instance"].Value<JArray>();
      try
      {
        using (StreamWriter sw = new StreamWriter(file))
        {
          for (int i = 0; i < productList.Count; i++)
            sw.WriteLine(productList[i]["@name"].Value<string>());
        }
        this.ReadDptProductDictionary();
      }
      catch (System.Exception e)
      {
        throw new System.Exception(file, e);
      }
    }
  }
}
