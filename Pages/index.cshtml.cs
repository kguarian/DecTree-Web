using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NorthwindWeb.Backend;

namespace NorthwindWeb.Pages
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        string DataPath = System.IO.Path.GetFullPath("./Pages/Data/data.dec");
        string AddressPath = System.IO.Path.GetFullPath("./Pages/Data/addresses.txt");
        DecTree<string> DataHolder;
        public string DayName = System.DateTime.Now.ToString();
        public string output;

        //strings used in output processing which I'd like not to redefine.
        private string output_prefix = "Address: {";
        private string output_infix = "} Element: {";
        private string output_suffix = "}\n";
        //end of block

        public string subject_Address { get; set; }
        public string subject_Element { get; set; }

        public void OnGet()
        {
            
            ViewData["Title"] = "DecTree Database Demo!";
            using (System.IO.FileStream currFile = System.IO.File.OpenRead(DataPath))
                if (currFile.Length <= 1)
                {
                    currFile.Close();
                    DataHolder = new DecTree<string>();
                    DataHolder.Export(DataPath);
                }
                else
                {
                    DataHolder = DecTree<string>.Import(DataPath);
                    DataHolder.counter = DataHolder.Length();
                };
            DecString outputWriter = new DecString();
            long counter = DataHolder.counter;
            System.IO.FileStream fs = System.IO.File.OpenRead(AddressPath);

            DecString ds_address = new DecString();
            int currChar = (char)fs.ReadByte();
            while (currChar != -1)
            {
                if (currChar == '\n')
                {
                    outputWriter.AddString(output_prefix);
                    outputWriter.AddString(ds_address);
                    outputWriter.AddString(output_infix);
                    outputWriter.AddString(DataHolder.Get(ds_address.ToString()));
                    outputWriter.AddString(output_suffix);

                    ds_address.Clear();
                }
                else
                {
                    ds_address.AddChar((char) currChar);
                }
                currChar = fs.ReadByte();
            }
            fs.Close();
            output = outputWriter.ToString();
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                using (System.IO.FileStream currFile = System.IO.File.OpenRead(DataPath))
                    if (currFile.Length <= 1)
                    {
                        currFile.Close();
                        DataHolder = new DecTree<string>();
                        DataHolder.Export(DataPath);
                    }
                    else
                    {
                        DataHolder = DecTree<string>.Import(DataPath);
                        DataHolder.counter = DataHolder.Length();
                    };
                DataHolder = DecTree<string>.Import(DataPath);
                long counter = DataHolder.Length();
                try
                {
                    DataHolder.Get(subject_Address);
                    return RedirectToPage("/index");
                }
                catch (System.NullReferenceException)
                {
                    DataHolder.Add(subject_Address, subject_Element);
                    DataHolder.Export(DataPath);
                    System.IO.File.AppendAllText(AddressPath, $"{subject_Address}\n");
                }
            }
            return RedirectToPage("/index");
        }
    }
}