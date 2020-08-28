using Newtonsoft.Json;
using System;


public class ServiceProfile
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }
}

public class TariffProfile
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }
}

public class Sim
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("iccid")]
    public string iccid { get; set; }

    [JsonProperty("imsi")]
    public string imsi { get; set; }

    [JsonProperty("msisdn")]
    public string msisdn { get; set; }
}

public class IpAddressSpace
{

    [JsonProperty("id")]
    public int id { get; set; }
}

public class endpointResponse
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("tags")]
    public object tags { get; set; }

    [JsonProperty("created")]
    public DateTime created { get; set; }

    [JsonProperty("last_updated")]
    public DateTime last_updated { get; set; }

    [JsonProperty("status")]
    public Status status { get; set; }

    [JsonProperty("service_profile")]
    public ServiceProfile service_profile { get; set; }

    [JsonProperty("tariff_profile")]
    public TariffProfile tariff_profile { get; set; }

    [JsonProperty("sim")]
    public Sim sim { get; set; }

    [JsonProperty("imei")]
    public string imei { get; set; }

    [JsonProperty("ip_address")]
    public string ip_address { get; set; }

    [JsonProperty("ip_address_space")]
    public IpAddressSpace ip_address_space { get; set; }

    [JsonProperty("imei_lock")]
    public bool imei_lock { get; set; }
}

