
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace PesWeb.Service
{

using System;
    using System.Collections.Generic;
    
public partial class tblEvaluation
{

    public int Eva_ID { get; set; }

    public string EmployeeNO { get; set; }

    public string EvaluatorNO { get; set; }

    public Nullable<System.DateTime> Date { get; set; }

    public Nullable<int> Job_ID { get; set; }

    public string ProjectNO { get; set; }

    public string period { get; set; }

    public Nullable<int> PeriodID { get; set; }

    public Nullable<System.DateTime> StartEvaDate { get; set; }

    public Nullable<System.DateTime> FinishEvaDate { get; set; }

}

}
