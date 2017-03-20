﻿

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
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using System.Data.Entity.Core.Objects;
using System.Linq;


public partial class PSESEntities : DbContext
{
    public PSESEntities()
        : base("name=PSESEntities")
    {

    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        throw new UnintentionalCodeFirstException();
    }


    public virtual DbSet<tblEvaluation> tblEvaluation { get; set; }

    public virtual DbSet<tblHeaderBot> tblHeaderBot { get; set; }

    public virtual DbSet<tblHeaderJob> tblHeaderJob { get; set; }

    public virtual DbSet<tblHeaderMid> tblHeaderMid { get; set; }

    public virtual DbSet<tblHeaderTop> tblHeaderTop { get; set; }

    public virtual DbSet<tblPosition> tblPosition { get; set; }

    public virtual DbSet<tblScore> tblScore { get; set; }

    public virtual DbSet<Period> Period { get; set; }

    public virtual DbSet<tblProjectEmployee> tblProjectEmployee { get; set; }

    public virtual DbSet<tblProjectMember> tblProjectMember { get; set; }

    public virtual DbSet<tblProject> tblProject { get; set; }

    public virtual DbSet<tblPart2Master> tblPart2Master { get; set; }

    public virtual DbSet<tblTitle> tblTitle { get; set; }

    public virtual DbSet<tblEmployee> tblEmployee { get; set; }

    public virtual DbSet<tblOrganization> tblOrganization { get; set; }

    public virtual DbSet<tblHeaderLevel> tblHeaderLevel { get; set; }

    public virtual DbSet<tblHeader> tblHeader { get; set; }

    public virtual DbSet<tblHeaderAdditional> tblHeaderAdditional { get; set; }

    public virtual DbSet<tbllHeaderPosition> tbllHeaderPosition { get; set; }


    public virtual int CreateForm(string empID, string evaluator, Nullable<int> jobID, string projectNO)
    {

        var empIDParameter = empID != null ?
            new ObjectParameter("EmpID", empID) :
            new ObjectParameter("EmpID", typeof(string));


        var evaluatorParameter = evaluator != null ?
            new ObjectParameter("Evaluator", evaluator) :
            new ObjectParameter("Evaluator", typeof(string));


        var jobIDParameter = jobID.HasValue ?
            new ObjectParameter("JobID", jobID) :
            new ObjectParameter("JobID", typeof(int));


        var projectNOParameter = projectNO != null ?
            new ObjectParameter("ProjectNO", projectNO) :
            new ObjectParameter("ProjectNO", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CreateForm", empIDParameter, evaluatorParameter, jobIDParameter, projectNOParameter);
    }


    public virtual ObjectResult<Nullable<int>> getEvaID(string evaluatorNO, string employeeNO, string projectNO)
    {

        var evaluatorNOParameter = evaluatorNO != null ?
            new ObjectParameter("EvaluatorNO", evaluatorNO) :
            new ObjectParameter("EvaluatorNO", typeof(string));


        var employeeNOParameter = employeeNO != null ?
            new ObjectParameter("EmployeeNO", employeeNO) :
            new ObjectParameter("EmployeeNO", typeof(string));


        var projectNOParameter = projectNO != null ?
            new ObjectParameter("ProjectNO", projectNO) :
            new ObjectParameter("ProjectNO", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("getEvaID", evaluatorNOParameter, employeeNOParameter, projectNOParameter);
    }


    public virtual ObjectResult<Nullable<int>> GetHeader2BYHJ(Nullable<int> hJ_ID)
    {

        var hJ_IDParameter = hJ_ID.HasValue ?
            new ObjectParameter("HJ_ID", hJ_ID) :
            new ObjectParameter("HJ_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetHeader2BYHJ", hJ_IDParameter);
    }


    public virtual ObjectResult<Nullable<int>> GetHeader3ByH2ID(Nullable<int> h2_ID)
    {

        var h2_IDParameter = h2_ID.HasValue ?
            new ObjectParameter("H2_ID", h2_ID) :
            new ObjectParameter("H2_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetHeader3ByH2ID", h2_IDParameter);
    }


    public virtual ObjectResult<Nullable<int>> GetHeaderByJobID()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("GetHeaderByJobID");
    }


    public virtual ObjectResult<GetEvaluationRequest_Result> GetEvaluationRequest(Nullable<int> evaluator)
    {

        var evaluatorParameter = evaluator.HasValue ?
            new ObjectParameter("evaluator", evaluator) :
            new ObjectParameter("evaluator", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetEvaluationRequest_Result>("GetEvaluationRequest", evaluatorParameter);
    }


    public virtual ObjectResult<SP_GetJobDetail_Result> SP_GetJobDetail()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetJobDetail_Result>("SP_GetJobDetail");
    }


    public virtual ObjectResult<SP_GetAllHeaderJob_Result> SP_GetAllHeaderJob()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetAllHeaderJob_Result>("SP_GetAllHeaderJob");
    }


    public virtual ObjectResult<Nullable<int>> SP_GetHeader2BYHJ(Nullable<int> hJ_ID)
    {

        var hJ_IDParameter = hJ_ID.HasValue ?
            new ObjectParameter("HJ_ID", hJ_ID) :
            new ObjectParameter("HJ_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_GetHeader2BYHJ", hJ_IDParameter);
    }


    public virtual ObjectResult<SP_GetAllHeaderByJobID_Result> SP_GetAllHeaderByJobID()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetAllHeaderByJobID_Result>("SP_GetAllHeaderByJobID");
    }


    public virtual ObjectResult<SP_HeaderTopByJob_Result> SP_HeaderTopByJob()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_HeaderTopByJob_Result>("SP_HeaderTopByJob");
    }


    public virtual ObjectResult<SP_GetHeaderMidByHeaderTopAndJobID_Result> SP_GetHeaderMidByHeaderTopAndJobID(Nullable<int> h1_ID, Nullable<int> jobID)
    {

        var h1_IDParameter = h1_ID.HasValue ?
            new ObjectParameter("H1_ID", h1_ID) :
            new ObjectParameter("H1_ID", typeof(int));


        var jobIDParameter = jobID.HasValue ?
            new ObjectParameter("JobID", jobID) :
            new ObjectParameter("JobID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetHeaderMidByHeaderTopAndJobID_Result>("SP_GetHeaderMidByHeaderTopAndJobID", h1_IDParameter, jobIDParameter);
    }


    public virtual ObjectResult<SP_HeaderTopByJobID_Result> SP_HeaderTopByJobID()
    {

        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_HeaderTopByJobID_Result>("SP_HeaderTopByJobID");
    }


    public virtual ObjectResult<Nullable<int>> SP_CreateEvaFormAndReturnPrimary(string empID, string evaluator, Nullable<int> jobID, string projectNO)
    {

        var empIDParameter = empID != null ?
            new ObjectParameter("EmpID", empID) :
            new ObjectParameter("EmpID", typeof(string));


        var evaluatorParameter = evaluator != null ?
            new ObjectParameter("Evaluator", evaluator) :
            new ObjectParameter("Evaluator", typeof(string));


        var jobIDParameter = jobID.HasValue ?
            new ObjectParameter("JobID", jobID) :
            new ObjectParameter("JobID", typeof(int));


        var projectNOParameter = projectNO != null ?
            new ObjectParameter("ProjectNO", projectNO) :
            new ObjectParameter("ProjectNO", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("SP_CreateEvaFormAndReturnPrimary", empIDParameter, evaluatorParameter, jobIDParameter, projectNOParameter);
    }


    public virtual int SP_InsertScore(Nullable<int> evaID, Nullable<int> h3_ID)
    {

        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        var h3_IDParameter = h3_ID.HasValue ?
            new ObjectParameter("H3_ID", h3_ID) :
            new ObjectParameter("H3_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_InsertScore", evaIDParameter, h3_IDParameter);
    }


    public virtual ObjectResult<SP_GetEvaHeaderByEvaID_Result> SP_GetEvaHeaderByEvaID(Nullable<int> evaID)
    {

        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetEvaHeaderByEvaID_Result>("SP_GetEvaHeaderByEvaID", evaIDParameter);
    }


    public virtual int SP_DeleteHeaderTop(Nullable<int> h1_ID, Nullable<int> jobID)
    {

        var h1_IDParameter = h1_ID.HasValue ?
            new ObjectParameter("H1_ID", h1_ID) :
            new ObjectParameter("H1_ID", typeof(int));


        var jobIDParameter = jobID.HasValue ?
            new ObjectParameter("JobID", jobID) :
            new ObjectParameter("JobID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_DeleteHeaderTop", h1_IDParameter, jobIDParameter);
    }


    public virtual int SP_DeleteHeaderBot(Nullable<int> h3_ID)
    {

        var h3_IDParameter = h3_ID.HasValue ?
            new ObjectParameter("H3_ID", h3_ID) :
            new ObjectParameter("H3_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_DeleteHeaderBot", h3_IDParameter);
    }


    public virtual int SP_DeleteHeaderMid(Nullable<int> h2_ID)
    {

        var h2_IDParameter = h2_ID.HasValue ?
            new ObjectParameter("H2_ID", h2_ID) :
            new ObjectParameter("H2_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_DeleteHeaderMid", h2_IDParameter);
    }


    public virtual int InsertHeaderBot(string text, Nullable<int> h2_ID)
    {

        var textParameter = text != null ?
            new ObjectParameter("Text", text) :
            new ObjectParameter("Text", typeof(string));


        var h2_IDParameter = h2_ID.HasValue ?
            new ObjectParameter("H2_ID", h2_ID) :
            new ObjectParameter("H2_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertHeaderBot", textParameter, h2_IDParameter);
    }


    public virtual int InsertHeaderMid(string text, Nullable<int> h1_ID, Nullable<int> jobID, string text_Eng)
    {

        var textParameter = text != null ?
            new ObjectParameter("Text", text) :
            new ObjectParameter("Text", typeof(string));


        var h1_IDParameter = h1_ID.HasValue ?
            new ObjectParameter("H1_ID", h1_ID) :
            new ObjectParameter("H1_ID", typeof(int));


        var jobIDParameter = jobID.HasValue ?
            new ObjectParameter("JobID", jobID) :
            new ObjectParameter("JobID", typeof(int));


        var text_EngParameter = text_Eng != null ?
            new ObjectParameter("Text_Eng", text_Eng) :
            new ObjectParameter("Text_Eng", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("InsertHeaderMid", textParameter, h1_IDParameter, jobIDParameter, text_EngParameter);
    }


    public virtual int SP_InsertHeaderJob(Nullable<int> jobID, Nullable<int> h1_ID)
    {

        var jobIDParameter = jobID.HasValue ?
            new ObjectParameter("JobID", jobID) :
            new ObjectParameter("JobID", typeof(int));


        var h1_IDParameter = h1_ID.HasValue ?
            new ObjectParameter("H1_ID", h1_ID) :
            new ObjectParameter("H1_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_InsertHeaderJob", jobIDParameter, h1_IDParameter);
    }


    public virtual ObjectResult<Nullable<int>> InsertHeaderTop(string text, string alias, string text_Eng)
    {

        var textParameter = text != null ?
            new ObjectParameter("Text", text) :
            new ObjectParameter("Text", typeof(string));


        var aliasParameter = alias != null ?
            new ObjectParameter("Alias", alias) :
            new ObjectParameter("Alias", typeof(string));


        var text_EngParameter = text_Eng != null ?
            new ObjectParameter("Text_Eng", text_Eng) :
            new ObjectParameter("Text_Eng", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("InsertHeaderTop", textParameter, aliasParameter, text_EngParameter);
    }


    public virtual ObjectResult<SP_InsertEvaluation_Result> SP_InsertEvaluation(string projectID, string evaluatorID, string employeeID, Nullable<int> jobID, string period, Nullable<int> periodID, Nullable<System.DateTime> startDate, Nullable<System.DateTime> finishDate)
    {

        var projectIDParameter = projectID != null ?
            new ObjectParameter("ProjectID", projectID) :
            new ObjectParameter("ProjectID", typeof(string));


        var evaluatorIDParameter = evaluatorID != null ?
            new ObjectParameter("EvaluatorID", evaluatorID) :
            new ObjectParameter("EvaluatorID", typeof(string));


        var employeeIDParameter = employeeID != null ?
            new ObjectParameter("EmployeeID", employeeID) :
            new ObjectParameter("EmployeeID", typeof(string));


        var jobIDParameter = jobID.HasValue ?
            new ObjectParameter("JobID", jobID) :
            new ObjectParameter("JobID", typeof(int));


        var periodParameter = period != null ?
            new ObjectParameter("period", period) :
            new ObjectParameter("period", typeof(string));


        var periodIDParameter = periodID.HasValue ?
            new ObjectParameter("periodID", periodID) :
            new ObjectParameter("periodID", typeof(int));


        var startDateParameter = startDate.HasValue ?
            new ObjectParameter("StartDate", startDate) :
            new ObjectParameter("StartDate", typeof(System.DateTime));


        var finishDateParameter = finishDate.HasValue ?
            new ObjectParameter("FinishDate", finishDate) :
            new ObjectParameter("FinishDate", typeof(System.DateTime));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_InsertEvaluation_Result>("SP_InsertEvaluation", projectIDParameter, evaluatorIDParameter, employeeIDParameter, jobIDParameter, periodParameter, periodIDParameter, startDateParameter, finishDateParameter);
    }


    public virtual int SP_DeleteEva(Nullable<int> evaID)
    {

        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_DeleteEva", evaIDParameter);
    }


    public virtual ObjectResult<SP_GetEmployeesListByPeriod_Result> SP_GetEmployeesListByPeriod(Nullable<int> periodID, string evaluatorID)
    {

        var periodIDParameter = periodID.HasValue ?
            new ObjectParameter("PeriodID", periodID) :
            new ObjectParameter("PeriodID", typeof(int));


        var evaluatorIDParameter = evaluatorID != null ?
            new ObjectParameter("EvaluatorID", evaluatorID) :
            new ObjectParameter("EvaluatorID", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetEmployeesListByPeriod_Result>("SP_GetEmployeesListByPeriod", periodIDParameter, evaluatorIDParameter);
    }


    public virtual ObjectResult<SP_GetEmployeeListByPeriodID_Result> SP_GetEmployeeListByPeriodID(Nullable<int> periodID, string evaluatorID)
    {

        var periodIDParameter = periodID.HasValue ?
            new ObjectParameter("PeriodID", periodID) :
            new ObjectParameter("PeriodID", typeof(int));


        var evaluatorIDParameter = evaluatorID != null ?
            new ObjectParameter("EvaluatorID", evaluatorID) :
            new ObjectParameter("EvaluatorID", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetEmployeeListByPeriodID_Result>("SP_GetEmployeeListByPeriodID", periodIDParameter, evaluatorIDParameter);
    }


    public virtual ObjectResult<SP_GetEvaListByEvaluatorID_Result> SP_GetEvaListByEvaluatorID(string evaluatorID)
    {

        var evaluatorIDParameter = evaluatorID != null ?
            new ObjectParameter("EvaluatorID", evaluatorID) :
            new ObjectParameter("EvaluatorID", typeof(string));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetEvaListByEvaluatorID_Result>("SP_GetEvaListByEvaluatorID", evaluatorIDParameter);
    }


    public virtual ObjectResult<SP_GetEvaDataByEvaID_Result> SP_GetEvaDataByEvaID(Nullable<int> evaID)
    {

        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetEvaDataByEvaID_Result>("SP_GetEvaDataByEvaID", evaIDParameter);
    }


    public virtual ObjectResult<SP_GetHeaderByPosition_Result> SP_GetHeaderByPosition(Nullable<int> positionNO, Nullable<int> evaID)
    {

        var positionNOParameter = positionNO.HasValue ?
            new ObjectParameter("PositionNO", positionNO) :
            new ObjectParameter("PositionNO", typeof(int));


        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_GetHeaderByPosition_Result>("SP_GetHeaderByPosition", positionNOParameter, evaIDParameter);
    }


    public virtual int SP_InsertAdditionalHeader(Nullable<int> parent, string text, string text_Eng, Nullable<int> evaID, string alias, Nullable<int> h_Level, Nullable<int> positionNo)
    {

        var parentParameter = parent.HasValue ?
            new ObjectParameter("parent", parent) :
            new ObjectParameter("parent", typeof(int));


        var textParameter = text != null ?
            new ObjectParameter("Text", text) :
            new ObjectParameter("Text", typeof(string));


        var text_EngParameter = text_Eng != null ?
            new ObjectParameter("Text_Eng", text_Eng) :
            new ObjectParameter("Text_Eng", typeof(string));


        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        var aliasParameter = alias != null ?
            new ObjectParameter("Alias", alias) :
            new ObjectParameter("Alias", typeof(string));


        var h_LevelParameter = h_Level.HasValue ?
            new ObjectParameter("H_Level", h_Level) :
            new ObjectParameter("H_Level", typeof(int));


        var positionNoParameter = positionNo.HasValue ?
            new ObjectParameter("PositionNo", positionNo) :
            new ObjectParameter("PositionNo", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_InsertAdditionalHeader", parentParameter, textParameter, text_EngParameter, evaIDParameter, aliasParameter, h_LevelParameter, positionNoParameter);
    }


    public virtual int SP_UpdateData(Nullable<int> evaID, Nullable<int> point, Nullable<int> h_ID)
    {

        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        var pointParameter = point.HasValue ?
            new ObjectParameter("point", point) :
            new ObjectParameter("point", typeof(int));


        var h_IDParameter = h_ID.HasValue ?
            new ObjectParameter("H_ID", h_ID) :
            new ObjectParameter("H_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_UpdateData", evaIDParameter, pointParameter, h_IDParameter);
    }


    public virtual int SP_UpdateAdditional(Nullable<int> evaID, Nullable<int> point, Nullable<int> h_ID)
    {

        var evaIDParameter = evaID.HasValue ?
            new ObjectParameter("EvaID", evaID) :
            new ObjectParameter("EvaID", typeof(int));


        var pointParameter = point.HasValue ?
            new ObjectParameter("point", point) :
            new ObjectParameter("point", typeof(int));


        var h_IDParameter = h_ID.HasValue ?
            new ObjectParameter("H_ID", h_ID) :
            new ObjectParameter("H_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_UpdateAdditional", evaIDParameter, pointParameter, h_IDParameter);
    }


    public virtual ObjectResult<SP_InsertHeader_Result> SP_InsertHeader(Nullable<int> parent, string text, string text_Eng, string alias, Nullable<int> h_Level, Nullable<int> positionNO)
    {

        var parentParameter = parent.HasValue ?
            new ObjectParameter("parent", parent) :
            new ObjectParameter("parent", typeof(int));


        var textParameter = text != null ?
            new ObjectParameter("Text", text) :
            new ObjectParameter("Text", typeof(string));


        var text_EngParameter = text_Eng != null ?
            new ObjectParameter("Text_Eng", text_Eng) :
            new ObjectParameter("Text_Eng", typeof(string));


        var aliasParameter = alias != null ?
            new ObjectParameter("Alias", alias) :
            new ObjectParameter("Alias", typeof(string));


        var h_LevelParameter = h_Level.HasValue ?
            new ObjectParameter("H_Level", h_Level) :
            new ObjectParameter("H_Level", typeof(int));


        var positionNOParameter = positionNO.HasValue ?
            new ObjectParameter("PositionNO", positionNO) :
            new ObjectParameter("PositionNO", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_InsertHeader_Result>("SP_InsertHeader", parentParameter, textParameter, text_EngParameter, aliasParameter, h_LevelParameter, positionNOParameter);
    }


    public virtual int SP_InsertHeaderPosition(Nullable<int> positionNo, Nullable<int> headerID)
    {

        var positionNoParameter = positionNo.HasValue ?
            new ObjectParameter("PositionNo", positionNo) :
            new ObjectParameter("PositionNo", typeof(int));


        var headerIDParameter = headerID.HasValue ?
            new ObjectParameter("HeaderID", headerID) :
            new ObjectParameter("HeaderID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_InsertHeaderPosition", positionNoParameter, headerIDParameter);
    }


    public virtual int SP_DeleteHeader(Nullable<int> h_ID)
    {

        var h_IDParameter = h_ID.HasValue ?
            new ObjectParameter("H_ID", h_ID) :
            new ObjectParameter("H_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_DeleteHeader", h_IDParameter);
    }


    public virtual int SP_DeleteHeaderAdditional(Nullable<int> h_ID)
    {

        var h_IDParameter = h_ID.HasValue ?
            new ObjectParameter("H_ID", h_ID) :
            new ObjectParameter("H_ID", typeof(int));


        return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_DeleteHeaderAdditional", h_IDParameter);
    }

}

}

