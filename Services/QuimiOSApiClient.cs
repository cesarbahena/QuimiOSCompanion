using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using QuimiOSCompanion.Models;

namespace QuimiOSCompanion.Services
{
    public class QuimiOSApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly CookieContainer _cookieContainer;
        private const string BaseUrl = "http://172.16.0.117";
        private const string Username = "cbahena";
        private const string Password = "alpe58";

        public QuimiOSApiClient()
        {
            _cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookieContainer,
                UseCookies = true
            };
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task<string> GetSessionCookieAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/");
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    return cookies.FirstOrDefault()?.Split(';')[0] ?? string.Empty;
                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<bool> AuthenticateAsync(string sessionCookie)
        {
            try
            {
                var loginData = new Dictionary<string, string>
                {
                    ["__LASTFOCUS"] = "",
                    ["__EVENTTARGET"] = "",
                    ["__EVENTARGUMENT"] = "",
                    ["__VIEWSTATE"] = "/wEPDwUJLTMxNDUzMzcyZBgBBR5fX0NvbnRyb2xzUmVxdWlyZVBvc3RCYWNrS2V5X18WAgUXTG9naW4xJExvZ2luSW1hZ2VCdXR0b24FN1Bhc3N3b3JkUmVjb3ZlcnkxJFVzZXJOYW1lQ29udGFpbmVySUQkU3VibWl0SW1hZ2VCdXR0b27JchTmM/smYgwfFRhXIoyaxmTSmg==",
                    ["__VIEWSTATEGENERATOR"] = "C2EE9ABB",
                    ["__EVENTVALIDATION"] = "/wEWBgLlpNzICgKUvNa1DwL666vYDAKnz4ybCALM1sHnCgK8lteMCvxsskT7SnXhPUIuQQ8jj9A2y/uL",
                    ["Login1$LoginButton"] = "Aceptar",
                    ["Login1$UserName"] = Username,
                    ["Login1$Password"] = Password,
                    ["PasswordRecovery1$UserNameContainerID$UserName"] = ""
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "/Login.aspx")
                {
                    Content = new FormUrlEncodedContent(loginData)
                };
                request.Headers.Add("Cookie", sessionCookie);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<GridData> LoadInventoryGridAsync(string sessionCookie, DateTime date)
        {
            try
            {
                var searchData = new Dictionary<string, string>
                {
                    ["ctl00$ContentMasterPage$ScriptManager1"] = "ctl00$ContentMasterPage$UpdatePanel1|ctl00$ContentMasterPage$btnBuscarEstudio",
                    ["__LASTFOCUS"] = "",
                    ["__EVENTTARGET"] = "",
                    ["__EVENTARGUMENT"] = "",
                    ["ctl00_treePrincipal_ExpandState"] = "eunnnnnnnnnnnnnunnnnnnnnnnnnnnunnnnunnnnnnnnnnnennnunnun",
                    ["ctl00_treePrincipal_SelectedNode"] = "ctl00_treePrincipalt48",
                    ["__VIEWSTATEGENERATOR"] = "81A526C4",
                    ["__EVENTVALIDATION"] = "/wEWMgKl4pa4DwL66ZujDwKP962oBQLLz7SHAgKgodW7AwLEoJ7pDgLFoJ7pDgLGoJ7pDgLHoJ7pDgLBoJ7pDgLcoJ7pDgLlwMuZCQKOrqqlCALvr6H0BQLur8H3BQLur5H0BQLvr+H3BQL3k7+XDQLqr6n0BQKdqZFgAumvkfQFAuivkfQFAuqvmfQFAu+vmfQFArvAiv0GAu+vrfQFAu6vzfcFAuiv4fcFAuqvrfQFAvKv4fcFAuivzfcFAumvmfQFAuqvkfQFAsn8xb0HAuuvwfcFAuuvzfcFAu6vofQFAqHX7NYMAuyv4fcFAuuvlfQFAsirxowDAqPFp7ACAsbE7OIPAsXE7OIPAoa3nbABAr3K1bwLAvmowt8FAsy51WsC1KqatgUC4+nN4gw6S2SJyBOscYw4C1/0ibj3kG7oUQ==",
                    ["ctl00$ContentMasterPage$txtFecDesde_MaskedEditExtender_ClientState"] = "",
                    ["ctl00$ContentMasterPage$txtDesdeB"] = date.ToString("dd/MM/yyyy"),
                    ["ctl00$ContentMasterPage$txtFecDesde"] = date.ToString("MM/dd/yyyy"),
                    ["ctl00$ContentMasterPage$txtFecHasta"] = date.ToString("MM/dd/yyyy"),
                    ["ctl00$ContentMasterPage$ddlSuc"] = "2",
                    ["ctl00$ContentMasterPage$cmbEquipo"] = "6",
                    ["ctl00$ContentMasterPage$cmbMesaOrdenac"] = "2",
                    ["ctl00$ContentMasterPage$hfActivo"] = "0",
                    ["ctl00$ContentMasterPage$hfCalcAuto"] = "1",
                    ["ctl00$ContentMasterPage$btnBuscarEstudio"] = "Buscar"
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "/Inventarios/ConsumoReacLabMasivo.aspx")
                {
                    Content = new FormUrlEncodedContent(searchData)
                };
                request.Headers.Add("Cookie", sessionCookie);

                var response = await _httpClient.SendAsync(request);
                var html = await response.Content.ReadAsStringAsync();

                return ParseGridData(html);
            }
            catch
            {
                return new GridData();
            }
        }

        private GridData ParseGridData(string html)
        {
            var gridData = new GridData
            {
                Rows = new Dictionary<string, GridRowData>(),
                Inventory = new Dictionary<string, int>()
            };

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var codeElements = doc.DocumentNode.SelectNodes("//*[contains(@class, 'wd090')]");
            if (codeElements == null)
                return gridData;

            foreach (var codeElement in codeElements)
            {
                var reagentCode = codeElement.InnerText.Trim();
                var elementId = codeElement.GetAttributeValue("id", "");

                if (elementId.Length < 40)
                    continue;

                var rowNumber = elementId.Substring(38, 2);

                var stockElementId = $"ctl00_ContentMasterPage_grdConsumo_ctl{rowNumber}_lblExistFinal";
                var stockElement = doc.DocumentNode.SelectSingleNode($"//*[@id='{stockElementId}']");

                var productIdElementId = $"ctl00_ContentMasterPage_grdConsumo_ctl{rowNumber}_hfIDProducto";
                var productIdElement = doc.DocumentNode.SelectSingleNode($"//*[@id='{productIdElementId}']");

                var stock = stockElement != null && int.TryParse(stockElement.InnerText.Trim(), out var s) ? s : 0;
                var productId = productIdElement != null && int.TryParse(productIdElement.GetAttributeValue("value", "0"), out var id) ? id : 0;

                gridData.Rows[reagentCode] = new GridRowData
                {
                    RowNumber = rowNumber,
                    ProductId = productId
                };

                gridData.Inventory[reagentCode] = stock;
            }

            return gridData;
        }

        public async Task<bool> SubmitConsumptionAsync(
            string sessionCookie,
            DateTime date,
            List<ConsumptionData> consumptions,
            GridData gridData)
        {
            try
            {
                var formData = new Dictionary<string, string>();

                AddCommonData(formData);
                AddSearchParams(formData, date);
                AddInputValues(formData, consumptions, gridData.Rows);
                AddMetaValues(formData, gridData.Rows);

                var request = new HttpRequestMessage(HttpMethod.Post, "/Inventarios/ConsumoReacLabMasivo.aspx")
                {
                    Content = new FormUrlEncodedContent(formData)
                };
                request.Headers.Add("Cookie", sessionCookie);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private void AddCommonData(Dictionary<string, string> formData)
        {
            formData["ctl00$ContentMasterPage$ScriptManager1"] = "ctl00$ContentMasterPage$UpdatePanel1|ctl00$ContentMasterPage$btnGuardaMasivo";
            formData["__LASTFOCUS"] = "";
            formData["__EVENTTARGET"] = "";
            formData["__EVENTARGUMENT"] = "";
            formData["ctl00_treePrincipal_ExpandState"] = "eunnnnnnnnnnnnnunnnnnnnnnnnnnnnunnnnunnnnnnnnnnnennnunnun";
            formData["ctl00_treePrincipal_SelectedNode"] = "ctl00_treePrincipalt49";
            formData["__VIEWSTATEGENERATOR"] = "81A526C4";
            formData["ctl00$ContentMasterPage$txtFecDesde_MaskedEditExtender_ClientState"] = "";
            formData["ctl00$ContentMasterPage$ddlSuc"] = "2";
            formData["ctl00$ContentMasterPage$cmbEquipo"] = "6";
            formData["ctl00$ContentMasterPage$cmbMesaOrdenac"] = "2";
            formData["ctl00$ContentMasterPage$hfActivo"] = "0";
            formData["ctl00$ContentMasterPage$hfCalcAuto"] = "0";
            formData["ctl00$ContentMasterPage$grdConsumo$ctl01$chkProvAll"] = "on";
            formData["ctl00$ContentMasterPage$btnGuardaMasivo"] = "Guardar Consumo";
        }

        private void AddSearchParams(Dictionary<string, string> formData, DateTime date)
        {
            formData["ctl00$ContentMasterPage$txtDesdeB"] = date.ToString("dd/MM/yyyy");
        }

        private void AddInputValues(Dictionary<string, string> formData, List<ConsumptionData> consumptions, Dictionary<string, GridRowData> rows)
        {
            foreach (var consumption in consumptions)
            {
                if (!rows.TryGetValue(consumption.ReagentCode, out var rowData))
                    continue;

                var prefix = $"ctl00$ContentMasterPage$grdConsumo$ctl{rowData.RowNumber}";

                formData[$"{prefix}$txtPacientes"] = consumption.Px.ToString();
                formData[$"{prefix}$txtRepeticiones"] = consumption.Rep.ToString();
                formData[$"{prefix}$txtControlCapMGrd"] = consumption.QC.ToString();
                formData[$"{prefix}$txtCalibracionCapMGrd"] = consumption.Cal.ToString();
                formData[$"{prefix}$txtCancelacionCapMGrd"] = consumption.Canc.ToString();
                formData[$"{prefix}$cmbMotCancelacionGrd"] = consumption.Motivo;
                formData[$"{prefix}$hfIDProducto"] = rowData.ProductId.ToString();
            }
        }

        private void AddMetaValues(Dictionary<string, string> formData, Dictionary<string, GridRowData> rows)
        {
            foreach (var rowData in rows.Values)
            {
                var prefix = $"ctl00$ContentMasterPage$grdConsumo$ctl{rowData.RowNumber}";

                formData[$"{prefix}$chkQueProveedor"] = "on";
                formData[$"{prefix}$txtValidacionCapMGrd"] = "0";
                formData[$"{prefix}$txtSinIdentificarCapMGrd"] = "0";
                formData[$"{prefix}$cmbMotSinIdentificarGrd"] = "[Seleccione]";
            }
        }
    }

    public class GridData
    {
        public Dictionary<string, GridRowData> Rows { get; set; } = new Dictionary<string, GridRowData>();
        public Dictionary<string, int> Inventory { get; set; } = new Dictionary<string, int>();
    }
}
