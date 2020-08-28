using Newtonsoft.Json;

public class EsmeInterfaceType
{

    [JsonProperty("description")]
    public string description { get; set; }

    [JsonProperty("id")]
    public int id { get; set; }
}

public class BreakoutRegion
{

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("ip_address")]
    public string ip_address { get; set; }

    [JsonProperty("id")]
    public int id { get; set; }
}

public class serviceProfileResponse
{

    [JsonProperty("organisation_id")]
    public string organisation_id { get; set; }

    [JsonProperty("name")]
    public string name { get; set; }

    [JsonProperty("description")]
    public string description { get; set; }

    [JsonProperty("allowed_3g")]
    public string allowed_3g { get; set; }

    [JsonProperty("moc_callback_id")]
    public object moc_callback_id { get; set; }

    [JsonProperty("retail")]
    public string retail { get; set; }

    [JsonProperty("sms_p2p_int")]
    public string sms_p2p_int { get; set; }

    [JsonProperty("sms_p2p_ext")]
    public string sms_p2p_ext { get; set; }

    [JsonProperty("apply_quota")]
    public string apply_quota { get; set; }

    [JsonProperty("prepaid")]
    public string prepaid { get; set; }

    [JsonProperty("nipdp")]
    public string nipdp { get; set; }

    [JsonProperty("allowed_4g")]
    public string allowed_4g { get; set; }

    [JsonProperty("allowed_nb_iot")]
    public string allowed_nb_iot { get; set; }

    [JsonProperty("apply_sms_quota")]
    public string apply_sms_quota { get; set; }

    [JsonProperty("used_count")]
    public string used_count { get; set; }

    [JsonProperty("id")]
    public int id { get; set; }

    [JsonProperty("api_callback")]
    public object api_callback { get; set; }

    [JsonProperty("api_secret")]
    public object api_secret { get; set; }

    [JsonProperty("esme_interface_type")]
    public EsmeInterfaceType esme_interface_type { get; set; }

    [JsonProperty("breakout_region")]
    public BreakoutRegion breakout_region { get; set; }

    [JsonProperty("dns")]
    public object dns { get; set; }

    [JsonProperty("apply_data_quota")]
    public string apply_data_quota { get; set; }
}

