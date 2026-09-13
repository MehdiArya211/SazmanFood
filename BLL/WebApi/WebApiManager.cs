using BLL.Interface;
using DTO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using IdentityModel.Client;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.Bibliography;
using System.Text;
using DTO.WebApi;



namespace BLL
{


	public class WebApiManager : IWebApiManager
	{
		private string apiUrl = string.Empty;
		private string _apiPersonalUrl = string.Empty;
		private string apiOrganUrl = string.Empty;
		private string apiTashvighatUrl = string.Empty;
		private string apiTanbihatUrl = string.Empty;
		private string apiEntehgalatUrl = string.Empty;
		private string apitashilateMaskanUrl = string.Empty;
		private string apiTashilatDabirKhaneUrl = string.Empty;
		private string apiPersonFamilyUrl = string.Empty;
		private string apiFajrUrl = string.Empty;
		private string apiDastorUrl = string.Empty;
		private string apiTashilatDastorUrl = string.Empty;
		private string apiTashilatOtherUrl = string.Empty;
		private string apiExamUrl = string.Empty;
		private string _urlOrg = string.Empty;
		private string access_token = string.Empty;
		private readonly IConfiguration _configuration;
		private HttpClient _client;
		private string apiProvinceCityUrl = string.Empty;
		private string apiUrlSarbazMaher_Aja = string.Empty;



		public WebApiManager(IConfiguration configuration)
		{
			_configuration = configuration;
			//برای سرویس ارگانه
			apiOrganUrl = CustomSettings.Instance.ApiOrganUrl;
			_apiPersonalUrl = CustomSettings.Instance.ApiPersonelUrl;
			_urlOrg = CustomSettings.Instance.ApiOrganUrl;
			apiProvinceCityUrl = CustomSettings.Instance.ApiProvinceCityUrl;
			apiUrlSarbazMaher_Aja = CustomSettings.Instance.UrlSarbazMaher_Aja;


			_client = new HttpClient();

		}



		public List<OrganInfDTO> GetOrganInfo(string token)
		{
			_client.SetBearerToken(token);
			var result = _client.GetStringAsync(_urlOrg + "/GetOrganByCategoryCode/" + 7746).Result;
			List<OrganInfDTO> organ = JsonConvert.DeserializeObject<List<OrganInfDTO>>(result);
			return organ;


		}

		public PersonalInfDTO GetPersonalByPersonCode(string PersonCode, string token)
		{
			_client.SetBearerToken(token);
			var result = _client.GetStringAsync(_apiPersonalUrl + "/GetPersonalByPersonalCode/" + PersonCode).Result;
			var person = JsonConvert.DeserializeObject<PersonalInfDTO>(result);
			return person;
		}

		public OrganInfDTO GetOrganInfoByOrganId(int Id, string token)
		{
			_client.SetBearerToken(token);
			var result = _client.GetStringAsync(_urlOrg + "/GetOrganInfo/" + Id).Result;
			OrganInfDTO organ = JsonConvert.DeserializeObject<OrganInfDTO>(result);
			return organ;
		}

		public List<CityDTO> GetCityByProvinceId(int provinceId, string token)
		{
			_client.SetBearerToken(token);
			var result2 = _client.GetStringAsync(apiProvinceCityUrl + "/GetCityByProvinceId/" + provinceId).Result;
			//convert
			var Cities = JsonConvert.DeserializeObject<List<CityDTO>>(result2);
			return Cities;
		}

		public List<ProvinceDTO> GetProvince(string token)
		{
			_client.SetBearerToken(token);

			var result = _client.GetStringAsync(apiProvinceCityUrl + "/GetAllProvince/").Result;
			//convert
			var res = JsonConvert.DeserializeObject<List<ProvinceDTO>>(result);
			return res;
		}

		public string GetProvinceTitleByProvinceId(int provinceId, string token)
		{
			var province = GetProvince(token);
			var provinceTitle = province.Where(x => x.Id == provinceId).Select(x => x.Title).FirstOrDefault();

			return provinceTitle;
		}

		public string GetCityTitleByCityId(int cityId, int provinceId, string token)
		{
			var city = GetCityByProvinceId(provinceId, token);
			var cityTitle = city.Where(x => x.Id == cityId).Select(x => x.Title).FirstOrDefault();

			return cityTitle;
		}

		public string GetUnitDutyByUnitId(int unitId, string token)
		{
			var Org = GetOrgan(token);
			string orgTitle = Org.Where(f => f.Id == unitId).Select(x => x.UnitTitle).FirstOrDefault();


			return orgTitle;
		}

		public ApiResultOrganDto GetListOrganInfo(string token)
		{
			_client.SetBearerToken(token);
			var result = _client.GetStringAsync(_urlOrg + "/GetOmdOrgan/").Result;
			ApiResultOrganDto organ = JsonConvert.DeserializeObject<ApiResultOrganDto>(result);
			return organ;
		}


		public List<OrganInfoViewModel> GetOrgan(string token)
		{
			_client.SetBearerToken(token);
			var result = _client.GetStringAsync(_urlOrg + "/GetOrganByCategoryCode/" + 7746).Result;
			List<OrganInfoViewModel> organ = JsonConvert.DeserializeObject<List<OrganInfoViewModel>>(result);
			return organ;


		}

		//public List<OrganViewModelDto> GetListOrganInfoV1(string token)
		//{
		//    _client.SetBearerToken(token);
		//    var result = _client.GetStringAsync(_urlOrg + "/GetOmdOrgan/").Result;
		//    var organ = JsonConvert.DeserializeObject<List<OrganViewModelDto>>(result);
		//    return organ;
		//}

		public List<OrganViewModelDto> GetListOrganInfoV1(string token)
		{
			_client.SetBearerToken(token);
			var result = _client.GetStringAsync(_urlOrg + "/GetOrganByCategoryCode/" + 7746).Result;
			var organ = JsonConvert.DeserializeObject<List<OrganViewModelDto>>(result);
			return organ;
		}


		public List<OrganInfDTO> GetOrganByCategoryCode(string token)
		{
			string Code = "7750";
			_client.SetBearerToken(token);
			var result = _client.GetStringAsync(_urlOrg + "/GetOrganByCategoryCode/" + Code).Result;
			//List<OrganInfDTO> organ = JsonConvert.DeserializeObject<List<OrganInfDTO>>(result);
			List<OrganInfDTO> organ = JsonConvert.DeserializeObject<List<OrganInfDTO>>(result);

			return organ;
		}

        #region Gharargah - Organ


        /// <summary>
        /// دریافت لیست قرارگاه ها
        /// </summary>

        public List<OrganViewModelDto> GetGharargah(string token)
        {
            _client.SetBearerToken(token);

            var result = _client
                .GetStringAsync(_urlOrg + "/GetGararghah/")
                .Result;

            var gharargah =
                JsonConvert.DeserializeObject<List<OrganViewModelDto>>(result);

            return gharargah;
        }



        /// <summary>
        /// دریافت یگان های زیر مجموعه یک قرارگاه
        /// </summary>
        /// <param name="gharargahId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ApiResultOrganDto GetOrganByGharargahId(
            int gharargahId,
            string token)
        {

            _client.SetBearerToken(token);


            var result = _client
                .GetStringAsync(
                    _urlOrg + "/GetOmdOrgansByGhId/" + gharargahId
                )
                .Result;



            var organs =
                JsonConvert.DeserializeObject<ApiResultOrganDto>(result);



            return organs;

        }


        #endregion
    }
}

