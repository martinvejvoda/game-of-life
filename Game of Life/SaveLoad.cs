using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using System.Threading.Tasks;

namespace Game_of_Life
{
    class SaveLoad
    {
        public SaveLoad()
        {

        }

        public void Save(History history, string path)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(history.GetType());

                using (StreamWriter sw = new StreamWriter(path))
                {
                    serializer.Serialize(sw, history);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        public History Load(string path)
        {
            History history = new History();

            try
            {
                XmlSerializer serializer = new XmlSerializer(history.GetType());
                using (StreamReader sr = new StreamReader(path))
                {
                    history = (History)serializer.Deserialize(sr);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            return history;
        }
    }
}
