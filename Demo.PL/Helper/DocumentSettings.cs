namespace Demo.PL.Helper
{
    public static class DocumentSettings
    {
        public static string UploadFile(IFormFile file,string folderName="")
        {
            if (file is not null)
            {
                // 1. Get Located Folder Path
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", folderName);

                // 2. Get File Name & Make it Unique
                var fileName = $"{Guid.NewGuid()}-{file.FileName}";

                //3. Get File Path
                var filePath = Path.Combine(folderPath, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);

                file.CopyTo(fileStream);

                return fileName;
            }
            return "";
        }
        public static bool RemoveFile(string fileName,string folderName="")
        {      

            try
            {            
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files", folderName);
                var filePath = Path.Combine(folderPath, fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing file: {ex.Message}");
            }
            return false;
      
        }
    }
}
