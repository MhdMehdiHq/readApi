
using Newtonsoft.Json;


//برنامه یکم طول میکشه تا ران بشه 


HttpClient httpClient = new HttpClient();
string stringAPI = "https://api.wallex.ir/v1/currencies/stats";

await RuningInBackground(TimeSpan.FromSeconds(10), () => InitAsync());
async Task RuningInBackground(TimeSpan timeSpan, Action action)
{
    var periodicTimer = new PeriodicTimer(timeSpan);
    while (await periodicTimer.WaitForNextTickAsync())
    {
        action();
    }
}

async Task InitAsync()
{
    //وصل کردن ای پی آی با کلاینت
    HttpResponseMessage response = await httpClient.GetAsync(stringAPI);
    if (response.IsSuccessStatusCode)
    {
        string apiresponse = await response.Content.ReadAsStringAsync();

        //Deserialize the entire JSON response into ApiResponseWrapper
        ApiResponseWrapper apiWrapper = JsonConvert.DeserializeObject<ApiResponseWrapper>(apiresponse);

        // Access the data items within the ApiResponseWrapper
        List<ResultItem> resultItems = apiWrapper.Result;


        foreach (var item in resultItems)
        {
            Console.WriteLine($"Key: {item.key}");
            Console.WriteLine($"Name: {item.name_en}");
            Console.WriteLine($"rank: {item.rank}");
            Console.WriteLine($"price: {item.price}");
            Console.WriteLine($"daily high price: {item.daily_high_price}");
            Console.WriteLine($"daily low price: {item.daily_low_price}");
            Console.WriteLine($"percent change 1 hour: {item.percent_change_1h}");
            Console.WriteLine(item.prediction());
            //پیش بینی قیمت مثلا در چهار ساعت بعد 
            Console.WriteLine(item.prediction(4));
            Console.WriteLine();
        }
    }
}



Console.ReadKey();  

public class ApiResponseWrapper
{
    public List<ResultItem> Result { get; set; }
}

public class ResultItem
{
    public string key { get; set; }
    public string name_en { get; set; }
    public int? rank { get; set; }
    public float price { get; set; }
    public float? daily_high_price { get; set; }
    public float? daily_low_price { get; set; }
    public float? percent_change_1h { get; set; }

    //پیش بینی قیمت در یک ساعت بعد
    public string prediction()
    {
        double newPrice = 0;
        newPrice = price + (price * Convert.ToDouble(percent_change_1h) / 100);
        string output = $"Possible price in 1 hour is: {newPrice}";
        return output;
    }

    //پیش بینی قیمت در چند ساعت بعد. ساعت توسط عدد ورودی تعیین می شود
    public string prediction(int hour)
    {
        double? newPrice = 0;
        newPrice = (price + (price * hour * percent_change_1h / 100));
        string output = $"Possible price in {hour} hours is: {newPrice}";
        return output;
    }
}