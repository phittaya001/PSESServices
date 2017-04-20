using CrystalDecisions.CrystalReports.Engine;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;


namespace PESproj.Views.Control
{
    /// <summary>
    /// Summary description for Handler1
    /// </summary>
    public class Handler1 : IHttpHandler
    {

        //public void ProcessRequest(HttpContext context)
        //{
        //    context.Response.ContentType = "text/plain";
        //    context.Response.Write("Hello World");
        //}

        //public bool IsReusable
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        public void ProcessRequest (HttpContext context)
        {
            
            using (ReportDocument rpt = new ReportDocument())
            {
              //string strReportHeaderImageLocation = HttpContext.Current.Server.MapPath("~/Images/Report/report_header_test.jpg");

    string strReportName = "Billing";
    string strReportFileFullPath = HttpContext.Current.Server.MapPath("../Views/Test1.rpt");

    rpt.Load(strReportFileFullPath);
                
               // BillingService db = new BillingService();
   // List<SP_BLMR0200_BILLING_REPORT_Result> REPORT_MAIN_DATA = db.SP_BLMR0200_BILLING_REPORT(key);
                //for (int i = 0; i < 100; i++)
                //    REPORT_MAIN_DATA.Add(REPORT_MAIN_DATA.Last());


//                foreach (var total in REPORT_MAIN_DATA)
//                {
//                    total.THAIBAHT = ThaiBaht(Convert.ToString(total.BILLING_TOTAL_AMOUNT));
//                }
//rpt.SetDataSource(CommonUtils.ConvertDoListToDataTable<SP_BLMR0200_BILLING_REPORT_Result>(REPORT_MAIN_DATA));


                //String strCompanyName = "CTI LOGISTICS CO.,LTD.";
                //String strCompanyAddress1 = "อาคาร ซีทีไอทาวเวอร์  ชั้น 31 191/2-5";
                //String strCompanyAddress2 = "ถ. รัชดาภิเษก แขวงคลองเตย เขตคลองเตย กทม. 10110";

                //TextObject txtCompanyName;
                //TextObject txtCompanyAddress1;
                //TextObject txtCompanyAddress2;
                //txtCompanyName = rpt.ReportDefinition.ReportObjects["txtCompanyName"] as TextObject;
                //txtCompanyAddress1 = rpt.ReportDefinition.ReportObjects["txtCompanyAddress1"] as TextObject;
                //txtCompanyAddress2 = rpt.ReportDefinition.ReportObjects["txtCompanyAddress2"] as TextObject;

                //foreach (SP_BLMR0200_BILLING_REPORT_Result _Company in REPORT_MAIN_DATA)
                //{
                  
                //    if (!(String.IsNullOrEmpty(_Company.LOGO_PATH_FILE)))
                //    {
                //        strReportHeaderImageLocation = _Company.LOGO_PATH_FILE;
                //    }
                //}

                //txtCompanyName.Text = strCompanyName;
                //txtCompanyAddress1.Text = strCompanyAddress1;
                //txtCompanyAddress2.Text = strCompanyAddress2;
                //rpt.SetParameterValue("HeaderImageLocation", strReportHeaderImageLocation);

                rpt.DataSourceConnections.Clear();
                rpt.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, context.Response, false, strReportName);
                //var pdf = PdfResize(rpt);
                ////pdf.WriteTo(new FileStream("D:\\"+strReportName,FileMode.Create));

                //using (FileStream file = new FileStream("D:\\" + strReportName + ".pdf", FileMode.Create, System.IO.FileAccess.Write))
                //{

                //    // Writes a block of bytes to this stream using data from
                //    // a byte array.
                //    file.Write(pdf, 0, pdf.Length);

                //    // close file stream
                //    file.Close();
                //}
                //SentToClient(pdf, strReportName+".pdf");
                
            }
        }

        private void SentToClient(byte[] pdf, string fileName)
{
    HttpContext.Current.Response.Clear();
    HttpContext.Current.Response.BufferOutput = true;

    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
    HttpContext.Current.Response.ContentType = "application/octet-stream";
    HttpContext.Current.Response.BinaryWrite(pdf);

    HttpContext.Current.Response.End();
}

public byte[] ResizePDF(ReportDocument rpt)
{
    MemoryStream output = new MemoryStream();
    PdfReader reader = new PdfReader(rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));
    Document doc = new Document(PageSize.A4);
    Document.Compress = true;
    PdfWriter writer = PdfWriter.GetInstance(doc,
        output);
    doc.Open();
    PdfContentByte cb = writer.DirectContent;

    PdfImportedPage page;
    for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber++)
    {
        page = writer.GetImportedPage(reader, pageNumber);

        //if (page.Width <= page.Height)
        //    doc.SetPageSize(PageSize.A4);
        //else
        //    doc.SetPageSize(PageSize.LETTER.Rotate());
        doc.NewPage();

        cb.AddTemplate(page,
            doc.PageSize.Width,
            0, 0,
            doc.PageSize.Height,
            0, 0);
    }
    doc.Close();
    return output.ToArray();

}

private byte[] PdfResize(ReportDocument rpt)
{

    MemoryStream output = new MemoryStream();
    PdfReader resizeReader = new PdfReader(rpt.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat));

    Rectangle newRect = PageSize.A4;
    Document doc = new Document(newRect);
    Document.Compress = true;

    PdfWriter resizeWriter = PdfWriter.GetInstance(doc, output);
    doc.Open();

    PdfContentByte cb = resizeWriter.DirectContent;

    for (int pageNumber = 1; pageNumber <= resizeReader.NumberOfPages; pageNumber++)
    {
        PdfImportedPage page = resizeWriter.GetImportedPage(resizeReader, pageNumber);
        cb.AddTemplate(page, newRect.Width / resizeReader.GetPageSize(pageNumber).Width, 0, 0,
                       newRect.Height / resizeReader.GetPageSize(pageNumber).Height, 0, 0);
        doc.NewPage();
    }

    doc.Close();
    doc = null;

    return output.ToArray();
}

public bool IsReusable
{
    get
    {
        return false;
    }
}


    }
}