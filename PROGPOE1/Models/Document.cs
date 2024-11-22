namespace final.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string LecturerName { get; set; }
        public byte[] FileContent { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}