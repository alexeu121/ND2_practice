﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CarServiceLibrary_Karm
{
    public class TestService : ICarRepairService<WorkOrder, Service, Customer, CarParts, Car>
    {
      
        public bool Check(WorkOrder workOrder, Service service, Customer cust, CarParts cParts, Car car)
        {

            bool isValid = true;

            if (workOrder.OrderCar == null || workOrder.OrderCustomer == null || workOrder.ChosenServiceList.Count == 0)
            {
                Console.WriteLine("Some data was not entered");
            }

            if (workOrder.ChosenServiceList.)
            { }



            return isValid;

        }
    }
}