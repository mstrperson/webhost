using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using WebhostMySQLConnection.Web;


namespace WebhostMySQLConnection
{
    public class Signature
    {
        protected int FacultyId;
        public Image image
        {
            get;
            protected set;
        }

        public bool Exists
        {
            get
            {
                return image != null;
            }
        }

        public Signature(int facultyId)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Faculty faculty = db.Faculties.Where(f => f.ID == facultyId).Single();
                FacultyId = facultyId;
                
                if (faculty.SignatureData.Length <= 0)
                {
                    image = null;
                    return;
                }

                MemoryStream str = new MemoryStream();
                str.Seek(0, SeekOrigin.Begin);
                try
                {
                    str.Read(faculty.SignatureData, 0, faculty.SignatureData.Length);
                }
                catch
                {
                    image = null;
                    return;
                }
                MemoryStream mStream = new MemoryStream();
                for (int i = 0; i < faculty.SignatureData.Length; i++)
                {
                    mStream.WriteByte(faculty.SignatureData[i]);
                }

                try
                {
                    image = Image.FromStream(mStream);
                }
                catch(Exception e)
                {
                    MailControler.MailToWebmaster("Signature Image Failed To Load.", String.Format("Comment Letter Signature Image for {0} {1} failed to load properly.\n\nError: {2}", faculty.FirstName, faculty.LastName, e.Message));
                    image = null;
                }
            }
        }

        /// <summary>
        /// Save a new Signature straight from file content.
        /// </summary>
        /// <param name="inStream"></param>
        public void SaveNewSignature(Stream inStream)
        {
            image = System.Drawing.Image.FromStream(inStream);
            MemoryStream tmpStream = new MemoryStream();

            image.Save(tmpStream, System.Drawing.Imaging.ImageFormat.Png);
            tmpStream.Seek(0, SeekOrigin.Begin);
            byte[] imgBytes = new byte[tmpStream.Length];
            tmpStream.Read(imgBytes, 0, (int)tmpStream.Length);

            using (WebhostEntities db = new WebhostEntities())
            {
                Faculty faculty = db.Faculties.Where(f => f.ID == FacultyId).Single();
                faculty.SignatureData = imgBytes;
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Save a new Signature Image from a byte array.
        /// </summary>
        /// <param name="imgData"></param>
        public void SaveNewSignature(byte[] imgData)
        {
            using(WebhostEntities db =new WebhostEntities())
            {
                Faculty faculty = db.Faculties.Where(f => f.ID == FacultyId).Single();
                faculty.SignatureData = imgData;
                db.SaveChanges();
            }
            MemoryStream str = new MemoryStream();
            str.Seek(0, SeekOrigin.Begin);
            try
            {
                str.Read(imgData, 0, imgData.Length);
            }
            catch
            {
                image = null;
                return;
            }
            MemoryStream mStream = new MemoryStream();
            for (int i = 0; i < imgData.Length; i++)
            {
                mStream.WriteByte(imgData[i]);
            }

            image = Image.FromStream(mStream);
        }
    }
}
