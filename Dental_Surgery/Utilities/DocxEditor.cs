using Dental_Surgery.Pages.PageViewModels;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Spire.Doc;
using BookmarkEnd = DocumentFormat.OpenXml.Wordprocessing.BookmarkEnd;
using BookmarkStart = DocumentFormat.OpenXml.Wordprocessing.BookmarkStart;
using Document = Spire.Doc.Document;


namespace Dental_Surgery.Utilities
{
    public static class DocxEditor
    {

        public static void EditDocx(string filePath, BillViewModel bill, string newDocxFilePath)
        {
            // Copy the original file to the new file path
            File.Copy(filePath, newDocxFilePath, true);

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(newDocxFilePath, true))
            {
                // Insert text at the bookmark locations
                InsertTextAtBookmark(wordDoc, "PN", $"Patient Name: {bill.PatientName}");
                InsertTextAtBookmark(wordDoc, "PA", $"Patient Address: {bill.PatientAddress}");
                InsertTextAtBookmark(wordDoc, "PP", $"Patient Phone: {bill.PatientPhone}");
                InsertTextAtBookmark(wordDoc, "TreatmentCost", $"${bill.TreatmentCost}");
                if (bill.UnattendedAppointmentsCount > 0)
                {
                    InsertTextAtBookmark(wordDoc, "UnattendedCount", $"Missed Attendance Charges: {bill.UnattendedAppointmentsCount}");
                    InsertTextAtBookmark(wordDoc, "UnattendedCharge", $"${bill.UnattendedAppointmentCharge}");
                }
                
                InsertTextAtBookmark(wordDoc, "TName", $"{bill.TreatmentName}");
                InsertTextAtBookmark(wordDoc, "InvoiceDate", $"{DateTime.Now:dd/MM/yyyy}");
                InsertTextAtBookmark(wordDoc, "AppointmentDate", $"{bill.AppointmentDate:dd/MM/yyyy HH:mm}");
                var random = new Random();
                var invoiceNumber = random.Next(100000, 999999);
                InsertTextAtBookmark(wordDoc, "InvoiceNum", $"{invoiceNumber}");
                //calculate total cost from treatment cost and unattended charges
                InsertTextAtBookmark(wordDoc, "SubTotal", $"${bill.TotalCost}");
                //calculate tax from subtotal 
                var tax = bill.TotalCost * 0.15m;
                InsertTextAtBookmark(wordDoc, "Tax", $"${tax:F2}");
                InsertTextAtBookmark(wordDoc, "TotalCost", $"${(bill.TotalCost + tax):F2}");


                // Save changes
                wordDoc.MainDocumentPart.Document.Save();
            }
                

                // Export docx to pdf
                var pdfFilePath = newDocxFilePath.Replace(".docx", ".pdf");
                Document document = new Document();
                document.LoadFromFile(newDocxFilePath);
                document.SaveToFile(pdfFilePath, FileFormat.PDF);
            
        }

        private static void InsertTextAtBookmark(WordprocessingDocument wordDoc, string bookmarkName, string text)
        {
            var bookmarkStart = wordDoc.MainDocumentPart.Document.Body
                .Descendants<BookmarkStart>()
                .FirstOrDefault(b => b.Name == bookmarkName);

            if (bookmarkStart != null)
            {
                // Remove existing text within the bookmark
                var followingElements = bookmarkStart
                    .ElementsAfter()
                    .TakeWhile(e => !(e is BookmarkEnd))
                    .ToList();

                foreach (var elem in followingElements)
                {
                    elem.Remove();
                }

                // Insert new text
                Run run = new Run(new Text(text));
                bookmarkStart.Parent.InsertAfter(run, bookmarkStart);
            }
        }
    }
}