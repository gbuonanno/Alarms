using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Try1
{
    class Error
    {
        public int ID { get; set; }
        public int Key { get; set; } 
        public String Message { get; set; }
        public String Level { get; set; }
        public String Action { get; set; }
        public Double Time { get; set; }

        public string Date { get; set; }


    }
    class Error_list

    {
        
        public String Message { get; set; }
        public Double Time { get; set; }
        public int Count { get; set; }
        public int Level { get; set; }
        public double Total { get; set; }
        public int Code { get; set; }   

        public double Total_Stop()
        {
            double Total = Time * Count;
            return Total;
        }

    }

    class Errors_Excel
    {
        public int Key { get; set; }

        public String Message { get; set; }

        public String Action { get; set; }  
        
        public int Stop_Time { get; set; }   
    }
    
}

