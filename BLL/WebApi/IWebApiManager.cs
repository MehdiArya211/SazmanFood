using DTO;

namespace BLL.Interface
{
	public interface IWebApiManager
	{
		#region Organ

		OrganInfDTO GetOrganInfoByOrganId(int Id, string token);
		List<OrganInfDTO> GetOrganInfo(string token);
		string GetProvinceTitleByProvinceId(int provinceId, string token);
		string GetCityTitleByCityId(int cityId, int provinceId, string token);
		string GetUnitDutyByUnitId(int unitId, string token);


		ApiResultOrganDto GetListOrganInfo(string token);

		//  OrganInfoViewModel GetOrganInfoByOrganIdMain(int Id, string token);
		//  List<OrganInfDTO> GetOrganInfo(string token);
		//  ApiResultOrganDto GetListOrganInfo(string token);





		#endregion

		#region person
		PersonalInfDTO GetPersonalByPersonCode(string PersonCode, string token);


		#endregion
		List<CityDTO> GetCityByProvinceId(int provinceId, string token);

		List<ProvinceDTO> GetProvince(string token);

		List<OrganViewModelDto> GetListOrganInfoV1(string token);

		List<OrganInfDTO> GetOrganByCategoryCode(string token);



	}
}
