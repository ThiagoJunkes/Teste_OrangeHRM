using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using static OpenQA.Selenium.Interactions.WheelInputDevice;

namespace Teste_Selenium
{
    internal class Robo
    {
        public async Task Start()
        {
            var chromeOptions = new ChromeOptions();

            //chromeOptions.AddArgument("headless"); // roda o webdriver oculto
            chromeOptions.AddArgument("--ignore-certificate-errors-spki-list"); // ignora certificados adversos
            chromeOptions.AddArgument("--ignore-ssl-errorss"); // ignora confiança ssl
            chromeOptions.AddArgument("--ignore-certificate-error"); // ignora necessidade de certificado
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true"); // desativa bloqueio pop-up

            


            IWebDriver robo = new ChromeDriver(chromeOptions);
            robo.Manage().Window.Size = new Size(900, 1000);

            try
            {
                
                logarSite(robo); //#1

                string admin = collectAdmin(robo);

                excluirArquivos(@"C:\Users\9040\Documents\Projetos\Teste_OrangeHRM\Arquivos");

                //--------------DASHBOARD---------------\\
                printDashboard(robo);

                //------------Directory-------------\\
                printAllDirectorys(robo);

                //------------Vacancy--------------\\
                //await Console.Out.WriteAsync("Digite o nome da 'Vacancy': ");
                //string name = Console.ReadLine();
                string name = "Teste Automatico";

                createVacancy(robo, name, admin);

                //----------Deleta Vacancy---------\\

                int verificacao = verificaVacancy(robo, name);


                if (verificacao == 0)
                {
                    await Console.Out.WriteLineAsync("Vacancy not found!");
                }
                else
                {
                    printVacancy(robo, name, verificacao);
                    Wait(robo, 1);
                    deleteVacancy(robo, verificacao);
                }

                Wait(robo, 5);
                robo.Close();
                robo.Quit();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("Error: " + ex.Message);
            }
        }

        static void Wait(IWebDriver driver, int delay, int interval = 500)
        {
            var now = DateTime.Now;
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(delay));
            wait.PollingInterval = TimeSpan.FromMilliseconds(interval);
            wait.Until(wd => (DateTime.Now - now) - TimeSpan.FromSeconds(delay) > TimeSpan.Zero);
        }
        public static bool WaitElement(IWebDriver robo, string searchFor, int timespan = 5)
        {
            try
            {
                var wait = new WebDriverWait(robo, TimeSpan.FromSeconds(timespan));
                var result = wait.Until(drv => drv.FindElement(By.XPath(searchFor)));
                return result.Displayed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            
        }

        void logarSite(IWebDriver driver)
        {
            if (driver == null)
            {
                return;
            }

            driver.Navigate().GoToUrl("https://opensource-demo.orangehrmlive.com/web/index.php/auth/login");
            WaitElement(driver, "//*[@id='app']/div[1]/div/div[1]/div/div[2]/div[2]/div", 10);
            var login = driver.FindElement(By.XPath("//*[@id='app']/div[1]/div/div[1]/div/div[2]/div[2]/div"));

            string[] word = login.Text.Split('\n');

            string[] strings0 = word[0].ToString().Split(" ");
            string[] strings1 = word[1].ToString().Split(" ");

            string username = strings0[2];
            string password = strings1[2];

            var usernameLogin = driver.FindElement(By.XPath("//*[@id='app']/div[1]/div/div[1]/div/div[2]/div[2]/form/div[1]/div/div[2]/input"));
            usernameLogin.SendKeys(username);

            var passwordLogin = driver.FindElement(By.XPath("//*[@id='app']/div[1]/div/div[1]/div/div[2]/div[2]/form/div[2]/div/div[2]/input"));
            passwordLogin.SendKeys(password);

            var loginButton = driver.FindElement(By.XPath("//*[@id='app']/div[1]/div/div[1]/div/div[2]/div[2]/form/div[3]/button"));
            loginButton.Click();

            Console.WriteLine("Login Realizado!");

        }

        string collectAdmin(IWebDriver robo)
        {
            WaitElement(robo, "//*[@id='app']/div[1]/div[1]/aside/nav/div[2]/ul/li[1]/a");
            var adminButton = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[1]/aside/nav/div[2]/ul/li[1]/a"));
            adminButton.Click();

            WaitElement(robo, "//*[@id='app']/div[1]/div[2]/div[2]/div/div[1]/div[1]/div[2]/div[3]/button");
            var moreFilterButton = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[1]/div[1]/div[2]/div[3]/button"));
            moreFilterButton.Click();


            var userRoleOption = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[1]/div[2]/form/div[1]/div/div[2]/div/div[2]/div"));
            userRoleOption.Click();

            new Actions(robo).KeyDown(Keys.ArrowDown).KeyDown(Keys.Enter).Perform();


            robo.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(200);
            var searchButton = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[1]/div[2]/form/div[2]/button[2]"));
            searchButton.Click();

            Wait(robo, 1);

            var recordsFound = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[2]/div[2]/div/span"));
            Console.WriteLine(recordsFound.Text);

            Console.WriteLine("ADMIN:");
            int i = Convert.ToInt32(formataRecordsFound(recordsFound.Text.ToString()));
            string hiringManager = " ";
            for (int j = 1; j <= i; j++)
            {
                Console.Write(j);
                var rawRecords = robo.FindElements(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[2]/div[3]/div/div/div[" + j + "]"));
                foreach (var rawRecord in rawRecords)
                {
                    string[] pieces = rawRecord.Text.ToString().Split("\n");
                    //Console.WriteLine(": " + rawRecord.Text);
                    Console.WriteLine(": " + pieces[1]);
                    hiringManager = pieces[1];
                }
            }
            return hiringManager;
        }

        string formataRecordsFound(string records)
        {
            string[] qtdFound = records.Split(new char[] { ' ' });
            string[] format = qtdFound[0].Split(')');
            string[] format2 = format[0].Split('(');
            return format2[1];
        }

        void excluirArquivos(string path)
        {
            var dir = new DirectoryInfo(path);

            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            Console.WriteLine("Arquivos Excluidos");
        }

        void printDashboard(IWebDriver robo)
        {
            var dashboardOption = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[1]/aside/nav/div[2]/ul/li[8]"));
            dashboardOption.Click();

            Wait(robo, 7);

            var dashboard = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[7]/div/div[2]"));

            var elementScreenshot = (dashboard as ITakesScreenshot).GetScreenshot();
            elementScreenshot.SaveAsFile(@"C:\Users\9040\Documents\Projetos\Teste_OrangeHRM\Arquivos\1_dashboard.png");

            Console.WriteLine("Print Dashboard");
        }

        void printAllDirectorys(IWebDriver robo)
        {
            var directoryOption = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[1]/aside/nav/div[2]/ul/li[9]/a"));
            directoryOption.Click();

            Wait(robo, 3);

            var recordsFound = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[2]/div/div[1]/div/span"));
            
            Console.WriteLine(recordsFound.Text);

            int i = Convert.ToInt32(formataRecordsFound(recordsFound.Text.ToString()));
            if (i != 0)
            {
                var scrollBox = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[2]/div/div[2]"));
                WheelInputDevice.ScrollOrigin scrollOrigin = new WheelInputDevice.ScrollOrigin
                {
                    Element = scrollBox
                };

                for (int j = 0; j < (i / 10) + 1; j++)
                {
                    new Actions(robo)
                    .ScrollFromOrigin(scrollOrigin, 0, 2000)
                    .Perform();
                    Wait(robo, 1);
                }

                int scrolled = 3;
                //for (int j = 1; j <= i; j++)
                for (int j = i; j >= 1; j--)
                {
                    var record = robo.FindElements(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div[2]/div/div[2]/div/div[" + j + "]"));

                    foreach (var item in record)
                    {
                        string[] linhas = item.Text.ToString().Split("\n");
                        if (linhas.Length > 1)
                        {
                            Console.WriteLine("Print: " + linhas[0]);
                            string nome = linhas[0].Substring(0, linhas[0].IndexOf(' '));  //corta string
                            nome = nome.Trim();

                            //Wait(robo, 1);

                            var elementScreenshot = (item as ITakesScreenshot).GetScreenshot();
                            elementScreenshot.SaveAsFile(@"C:\Users\9040\Documents\Projetos\Teste_OrangeHRM\Arquivos\" + nome + "_" + j + ".png");

                            scrolled = 3;
                        }

                    }

                    if (j % 2 != 0)
                    {
                        new Actions(robo)
                        .ScrollFromOrigin(scrollOrigin, 0, -250)
                        .Perform();
                        scrolled--;

                        //Wait(robo, 1);
                    }
                    if (scrolled <= 0)
                    {
                        new Actions(robo)
                        .ScrollFromOrigin(scrollOrigin, 0, -250)
                        .Perform();
                        scrolled = 4;
                    }

                }
            }
        }

        void createVacancy(IWebDriver robo, string name, string admin)
        {
            var recruitmentOption = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[1]/aside/nav/div[2]/ul/li[5]"));
            recruitmentOption.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[1]/div[1]/header/div[2]/nav/ul/li[2]");
            var vacanciesOption = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[1]/header/div[2]/nav/ul/li[2]"));
            vacanciesOption.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[1]/button");
            var addVacancie = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[1]/button"));
            addVacancie.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div/form/div[1]/div[1]/div/div[2]");
            var nameField = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div/form/div[1]/div[1]/div/div[2]/input"));
            nameField.SendKeys(name);

            var titleField = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[2]/div[2]/div/div/form/div[1]/div[2]/div/div[2]/div/div/div[2]/i"));
            titleField.Click();
            new Actions(robo).KeyDown(Keys.ArrowDown).KeyDown(Keys.ArrowDown).KeyDown(Keys.Enter).Perform();

            var managerField = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div/form/div[3]/div[1]/div/div[2]/div/div/input"));
            managerField.SendKeys(admin);
            Wait(robo, 2);
            new Actions(robo).KeyDown(Keys.ArrowDown).KeyDown(Keys.Enter).Perform();

            var positionsfield = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div/form/div[3]/div[2]/div/div/div/div[2]/input"));
            positionsfield.SendKeys("3");

            var saveButton = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div/form/div[7]/button[2]"));
            saveButton.Click();

            Console.WriteLine("Criado: " + name);
        }

        int verificaVacancy(IWebDriver robo, string name)
        {
            var recruitmentOption = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[1]/aside/nav/div[2]/ul/li[5]"));
            recruitmentOption.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[1]/div[1]/header/div[2]/nav/ul/li[2]");
            var vacanciesOption = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[1]/header/div[2]/nav/ul/li[2]"));
            vacanciesOption.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[3]/div/div/div[1]");

            var elementosVacancies = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[3]/div/div"));
            int i = 0;
            string[] strings = elementosVacancies.Text.ToString().Split('\n');
            foreach (string s in strings)
            {
                string line = s.Trim();
                if (line == "Vacancy")
                {
                    i++;
                }
                if (line == name)
                {
                    return i;
                }
            }
            return 0;
        }

        void printVacancy(IWebDriver robo, string name, int numeroV)
        {
            var vacancy = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[3]/div/div/div[" + numeroV + "]"));

            new Actions(robo)
                .ScrollByAmount(0, numeroV*100)
                .Perform();
            Wait(robo, 1);
            var elementScreenshot = (vacancy as ITakesScreenshot).GetScreenshot();
            elementScreenshot.SaveAsFile(@"C:\Users\9040\Documents\Projetos\Teste_OrangeHRM\Arquivos\2_" + name + ".png");
            Console.WriteLine("Print Vacancy");
            //Wait(robo, 16);
        }

        void deleteVacancy(IWebDriver robo, int numeroV)
        {
            var recruitmentOption = robo.FindElement(By.XPath("//*[@id='app']/div[1]/div[1]/aside/nav/div[2]/ul/li[5]"));
            recruitmentOption.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[1]/div[1]/header/div[2]/nav/ul/li[2]");
            var vacanciesOption = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[1]/header/div[2]/nav/ul/li[2]"));
            vacanciesOption.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[3]/div/div/div[1]");

            var vacancy = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[3]/div/div/div["+ numeroV + "]"));
            Console.WriteLine("Excluindo Vacancy: ");
            Console.WriteLine(vacancy.Text);
            Console.WriteLine("------------------");

            var deleteButton = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[1]/div[2]/div[2]/div/div[2]/div[3]/div/div/div["+ numeroV + "]/div/div/div[1]/div[2]/div/div/button[1]"));
            deleteButton.Click();

            WaitElement(robo, "//*[@id=\"app\"]/div[3]/div/div/div/div[3]/button[2]");
            var confirmation = robo.FindElement(By.XPath("//*[@id=\"app\"]/div[3]/div/div/div/div[3]/button[2]"));
            confirmation.Click();

            Console.WriteLine("Vacancy Deletada");
        }
    }
}
