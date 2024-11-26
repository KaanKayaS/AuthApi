using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibary.Dtos
{
    public class ErrorDto
    {
        public List<String> Errors { get; private set; }= new List<String>();

        public bool IsShow { get; private set; }   // hatayı kullanıcıyamı göstercez yoksa yazılımcıyamı onun için bool bir değer. true ise kullanıcıya

    

        public ErrorDto(string error , bool isShow)
        {
            Errors.Add(error);
            IsShow = isShow;   
        }

        public ErrorDto(List<string>errors,bool isShow)
        {
            Errors = errors;
            IsShow = isShow;  
        }
    }
}
