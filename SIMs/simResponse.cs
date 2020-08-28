
using Newtonsoft.Json;
using System;

public class Status
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("description")]
    public string description { get; set; }
}

public class Country
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }
}

public class CustomerOrg
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("country")]
    public Country country { get; set; }
}

public class IssuerOrg
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("country")]
    public Country country { get; set; }
    }

    public class Endpoint
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("imei")]
    public string imei { get; set; }

    [JsonProperty("created")]
    public DateTime created { get; set; }

    [JsonProperty("last_updated")]
    public DateTime last_updated { get; set; }

    [JsonProperty("organisation_id")]
    public int organisation_id { get; set; }

    [JsonProperty("service_profile_id")]
    public int service_profile_id { get; set; }

    [JsonProperty("tariff_profile_id")]
    public int tariff_profile_id { get; set; }

    [JsonProperty("tags")]
    public object tags { get; set; }

    [JsonProperty("ip_address")]
    public string ip_address { get; set; }

    [JsonProperty("ip_address_space_id")]
    public int ip_address_space_id { get; set; }
}

public class Formfactor
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }
}

public class Manufacturer
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }
}

public class Model
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("description")]
    public string description { get; set; }

    [JsonProperty("memory_size")]
    public int memory_size { get; set; }

    [JsonProperty("formfactor")]
    public Formfactor formfactor { get; set; }

    [JsonProperty("manufacturer")]
    public Manufacturer manufacturer { get; set; }
}

public class simResponse
{

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("iccid")]
    public string iccid { get; set; }

    [JsonProperty("production_date")]
    public DateTime production_date { get; set; }

    [JsonProperty("activation_date")]
    public DateTime activation_date { get; set; }

    [JsonProperty("status")]
    public Status status { get; set; }

    [JsonProperty("customer_org")]
    public CustomerOrg customer_org { get; set; }

    [JsonProperty("issuer_org")]
    public IssuerOrg issuer_org { get; set; }

    [JsonProperty("endpoint")]
    public Endpoint endpoint { get; set; }

    [JsonProperty("imsi")]
    public string imsi { get; set; }

    [JsonProperty("msisdn")]
    public string msisdn { get; set; }

    [JsonProperty("model")]
    public Model model { get; set; }
}

