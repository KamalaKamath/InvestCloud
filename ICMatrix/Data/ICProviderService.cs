using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using ICMatrix.Utils;

namespace ICMatrix.Data
{
    public class ICProviderService
    {
        readonly string baseUrl;
        readonly string initUrl;
        readonly string validateUrl;
        bool configured = false;
        public ICProviderService()
        {
            baseUrl = ConfigurationManager.AppSettings["IcApi_BaseUrl"];
            initUrl = ConfigurationManager.AppSettings["IcApi_InitUrl"];
            validateUrl = ConfigurationManager.AppSettings["IcApi_ValidateUrl"];
            if (!string.IsNullOrEmpty(baseUrl) && !string.IsNullOrEmpty(initUrl) && !string.IsNullOrEmpty(validateUrl))
            {
                configured = true;
            }
        }
        public bool Configured
        {
            get { return configured; }
        }
        public async Task<InitResponse> InitDataSet(int matrixSize)
        {
            var response = new InitResponse
            {
                Success = false
            };
            var json = await HttpHelper.GetDataAsync(baseUrl + initUrl + matrixSize.ToString());  //full init url 
            response = JsonConvert.DeserializeObject<InitResponse>(json);
            return response;
        }
        public async Task<Tuple<DataResponse, int, bool>> GetDataSetAsync(string datasetName, string type, int index, bool colOrRow)
        {
            DataResponse response = new DataResponse
            {
                Success = false
            };

            string datasetUrl = $"{datasetName}/{type}/{index}";
            if (configured)
            {
                var content = await HttpHelper.GetDataAsync(baseUrl + datasetUrl);  //full get url 
                response = JsonConvert.DeserializeObject<DataResponse>(content);
            }
            return new Tuple<DataResponse, int, bool>(response, index, colOrRow);
        }
        public DataResponse GetDataSet(string datasetName, string type, int index)
        {
            DataResponse response = new DataResponse
            {
                Success = false
            };

            string datasetUrl = $"{datasetName}/{type}/{index}";
            if (configured)
            {
                var content = HttpHelper.GetData(baseUrl + datasetUrl);  //full get url 
                response = JsonConvert.DeserializeObject<DataResponse>(content);
            }
            return response;
        }

        public ValidateResponse ValidateDataSet(string data)
        {
            ValidateResponse response = new ValidateResponse
            {
                Success = false
            };
            if (configured)
            {
                var content = HttpHelper.PostData(baseUrl + validateUrl, data);
                response = JsonConvert.DeserializeObject<ValidateResponse>(content);
            }

            return response;
        }
 
    }
}
