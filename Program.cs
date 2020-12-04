using RPTLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Resources;
using System.Collections;
using System.Reflection;

namespace ReportTest1
{
    static class ToDebug
    {
        public static string ToDebugString<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
        }
    }
    class Program
    {

        

        public static string numGuid = "A78A";                         // первые четыре знака номера партии
       
        static string Server;
        static Dictionary<string, string> param;

        public static void log(string message)
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine(message);

            }
        }

        public string ReadResource(string name)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
           
            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        static void Main(string[] args)
        {



            /*Program p = new Program();

            string[] res = p.GetType().Assembly.GetManifestResourceNames();

            string result = p.ReadResource("TEST_HTML");*/
           




            Dictionary<string, string> config = DFReport.getConfig();                    // Получение конфигурации
            Server = config["Server"];
            string error;

            if (args.Length == 0) return;
            String tid = args[0];
            // Если пустой tid
            if (tid.Length != 16)
            {
                string msg = "ERROR: tid empty";
                byte[] buf = Encoding.UTF8.GetBytes(msg);
                bool ret = DFReport.putData(Server, tid, buf, out error);
                Environment.Exit(0);
            }
            log(Server);

            try
            {
                param = DFReport.getParam(Server, tid); // Получение параметров документа ключ значение
                if (param.Count == 0)
                {
                    string msg = "ERROR: param empty";
                    byte[] buf = Encoding.UTF8.GetBytes(msg);
                    bool ret = DFReport.putData(Server, tid, buf, out error);
                    Environment.Exit(0);
                }
            }
            catch (Exception gk)
            {
                    byte[] buf = Encoding.UTF8.GetBytes(gk.Message);
                    bool ret = DFReport.putData(Server, tid, buf, out error);
                    Environment.Exit(0);
            }
            log(param["type"]);

                // Передача html ==========================================================
                if (String.Compare(param["type"], "html") == 0)
                {
                    try
                    {
                    log("start send html");
                    string html = "<html><body><div>Содержание документа HTML5</div></body></html>";
                    byte[] buf = Encoding.UTF8.GetBytes(html);
                    bool ret = DFReport.putData(Server, tid, buf, out error);
                    //Thread.Sleep(2000);
                    if (ret == false)
                    {
                        log(error);
                    }                    }
                    catch(Exception ex) { log(ex.Message); ;  Environment.Exit(0); }
                log("end send html");
                Environment.Exit(0);
                }

            log("error_step");
                // Формирование документа ===================================================

                /*{



                    string error;
                    byte[] ddd = Encoding.UTF8.GetBytes(tid);
                    bool ret = DFReport.putData(server, tid, ddd, out error);
                    byte[] ddff = Encoding.UTF8.GetBytes(error);
                    FileStream fsfg = new FileStream("TEST_Tid.txt", FileMode.Create);  // .pdf, .xls, .doc
                    fsfg.Write(ddff, 0, ddff.Length);
                }

            }
            catch (Exception j)
            {
                byte[] ddff = Encoding.UTF8.GetBytes(j.ToString());
                FileStream fsfg = new FileStream("TEST_EXe.txt", FileMode.Create);  // .pdf, .xls, .doc
                fsfg.Write(ddff, 0, ddff.Length);
            }

            if (1 == 1)
            {
                Thread.Sleep(1000);
                Environment.Exit(0);
            }*/



         

            // получаем сведения по партии вагонов
            /*string sqlExpressionPart = $"SELECT part_id, oper, num_izm, start_time," +
                $" end_time FROM tb_part WHERE part_id LIKE '{numGuid}%'";*/

            // получаем 25 вагонов
            /*string sqlExpressionCar = $"SELECT car.part_id, car.car_id, num, car.tara, car.tara_e," +
                $" car.right_truck, car.brutto, car.netto, car.weighing_time, car.carrying_e," +
                $" car.att_time, car.left_truck, cont.name as shipper, cons.name as consigner," +
                $" mat.name as mat FROM tb_car as car left join sp_contractor as cont " +
                $" on car.shipper = cont.contractor_id left join sp_contractor as cons " +
                $" on car.consigner = cons.contractor_id left join sp_mat as mat " +
                $" on car.mat = mat.mat_id where car.part_id LIKE '{numGuid}%' and car.att_code in (1, 2)";*/

            //Dictionary<string, string> param = DFReport.getParam(server, tid); // Получение параметров документа ключ значение
            //  <"type","например pdf"> приходит от веб клиента

            ///// кирилу для проверки
            
            Dictionary<string, DataTable> DataSets = new Dictionary<string, DataTable>();

            // Создаем таблицу main_info
            DataTable mainInfo = new DataTable("main_info");
            mainInfo.Columns.Add(new DataColumn("num_izm", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("start_date", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("start_time", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("fraction", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("shipper", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("consignee", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("weigher", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("error", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("oper_name", Type.GetType("System.String")));
            mainInfo.Columns.Add(new DataColumn("date", Type.GetType("System.String")));

            // Заполняем таблицу mainInfo
            DataRow row = mainInfo.NewRow();
            row["num_izm"] = "3459";
            row["start_date"] = "05.06.2020";
            row["start_time"] = "12:10:04";
            row["fraction"] = "орех";
            row["shipper"] = "Гурьевский рудник";
            row["consignee"] = "ЕВРАЗ-Руда";
            row["weigher"] = "Веста-СД 100 - 1/2 Ф №369";
            row["error"] = "+/- 2.0%";
            row["oper_name"] = "Тарасюк Т.И.";
            row["date"] = (DateTime.Now).ToString();
            mainInfo.Rows.Add(row);

            // Добавляем таблицы в словарь
            DataSets["main_info"] = mainInfo;

            // Создаем таблицу cars_list
            DataTable carsList = new DataTable("cars_list");
            carsList.Columns.Add(new DataColumn("car_id", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("weighing_time", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("num", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("tara", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("brutto", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("netto", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("carrying", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("left_truck", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("right_truck", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("error_k", Type.GetType("System.String")));
            carsList.Columns.Add(new DataColumn("error_truck", Type.GetType("System.String")));

            // Заполняем таблицу carsList
            for (int i = 0; i < 25; i++)
            {
                DataRow row_1 = carsList.NewRow();
                row_1["car_id"] = (1 + i).ToString();
                row_1["weighing_time"] = $"12:{4 + i}:05";
                row_1["num"] = "65498563";
                row_1["tara"] = "25.6";
                row_1["brutto"] = "55.7";
                row_1["netto"] = "65.1";
                row_1["carrying"] = "72.5";
                row_1["left_truck"] = "46.6";
                row_1["right_truck"] = "45.5";
                row_1["error_k"] = "-1.45";
                row_1["error_truck"] = "0.65";
                carsList.Rows.Add(row_1);
            }
            // Добавляем таблицу в словарь
            DataSets["cars_list"] = carsList;

            try // Формируем документ
            {
                string rdl = File.ReadAllText("PlumbLineProtocol.rdl", Encoding.GetEncoding(866));
                byte[] buf = DFReport.build_doc(DataSets, rdl, param["type"]);

                //FileStream fs = new FileStream("output.xls", FileMode.Create);  // .pdf, .xls, .doc
                //fs.Write(buf, 0, buf.Length);
                
                DFReport.putData(Server, tid, buf, out error);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string msg = "ERROR: " + ex.Message;
                byte[] buf = Encoding.UTF8.GetBytes(msg);
                DFReport.putData(Server, tid, buf, out error);                
            }

            /*
            using (SqlConnection connection = new SqlConnection(ConnectionString)) // делаем подключение
            {
                Console.WriteLine("******************** сведения о подключении *******************************");
                Console.WriteLine();
                connection.Open();
                Console.WriteLine("Подключение открыто");
                Console.WriteLine("Свойства подключения:");
                Console.WriteLine("\tСтрока подключения: {0}", connection.ConnectionString);
                Console.WriteLine("\tБаза данных: {0}", connection.Database);
                Console.WriteLine("\tСервер: {0}", connection.DataSource);
                Console.WriteLine("\tВерсия сервера: {0}", connection.ServerVersion);
                Console.WriteLine("\tСостояние: {0}", connection.State);
                Console.WriteLine("\tWorkstationld: {0}", connection.WorkstationId);
                Console.WriteLine("***********************************************************************");
                Console.WriteLine();



                //Console.Read();
                
                SqlCommand command1 = new SqlCommand(sqlExpressionPart, connection); // делаем команду
                using (SqlDataReader reader = command1.ExecuteReader()) // класс для чтения строк из патока 
                {
                    if (reader.HasRows) // если есть данные
                    {
                        while (reader.Read()) // построчно считываем данные
                        {
                            DataRow row = mainInfo.NewRow();
                            //row.Field<string>["num_metering"] = reader.GetString;
                            //row["num_metering"] = reader.GetGuid(0).ToString();
                            Guid gdd = reader.GetGuid(0);
                            object part_id = reader.GetValue(0);
                            object oper = reader.GetValue(1);
                            object num_izm = reader.GetValue(2);
                            object start_time = reader.GetValue(3);
                            object end_time = reader.GetValue(4);
                            object num_metering = reader.GetValue(5);*/
            /*
                                        // заносим значения в объект part
                                        part.Part_id = reader.GetGuid(0);
                                        part.Oper = reader.GetString(1);
                                        part.Num_izm = reader[2] as int?;
                                        part.Start_time = reader.GetDateTime(3);
                                        part.End_time = reader.GetDateTime(4);
                                        part.Num_metering = reader[5] as int?;

                                        Console.WriteLine("{0} \t{1} \t{2} \t{3} \t{4} \t{5}", part_id, oper, num_izm, start_time, end_time, num_metering);
                                        Console.WriteLine();
                                        Console.WriteLine();
                                        Console.WriteLine("************** сведения о партии ***************************************************");
                                        Console.WriteLine($"{part.Part_id}, {part.Oper}, {part.Num_izm.ToString()}," +
                                            $" {part.Start_time.ToString()}, {part.End_time.ToString()}, {part.Num_izm.ToString()} ");
        }
    }
}*/
        } 
    }
    }

