using CV_Maker.Models;

namespace CV_Maker.DTOs
{
    public class FileTabDto
    {
        public string FileName { get; set; }
        public CV CV { get; set; }
        public bool HasUnsavedChanges { get; set; }

        public FileTabDto(string fileName, CV cv)
        {
            FileName = fileName;
            CV = cv;
        }
    }
}
