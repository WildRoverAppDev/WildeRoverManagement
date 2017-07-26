using System;
using System.Collections.Generic;
using System.Text;

namespace WildeRoverMgmtApp.Models
{
    interface IOrderable
    {
        //void SetOrderAmount(int count);
        string ToStringOrder();
    }
}
