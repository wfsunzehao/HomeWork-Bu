// See https://aka.ms/new-console-template for more information
using GrandPrix;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrandPrix
{
    class Venue
    {
        public string venueName { get; set; }
        public int noOfLaps { get; set; }
        public int averageLapTime { get; set; }
        public double chanceOfRain { get; set; }
    }

    class Driver
    {
        public string name { get; set; }
        public int Ranking { get; set; }
        public string specialSkill { get; set; }
        public bool eligibleToRace { get; set; }
        public int accumulatedScore { get; set; }
        public int accumulatedTime { get; set; }
    }

    class ListOfVenues
    {
        public List<Venue> venues { get; set; }
    }

    class ListOfDrivers
    {
        public List<Driver> drivers { get; set; }
    }

    class Championship
    {
        public ListOfDrivers drivers;
        public ListOfVenues venues;

        public Championship(ListOfDrivers drivers, ListOfVenues venues)
        {
            this.drivers = drivers;
            this.venues = venues;
        }

        public void RunChampionship()
        {
            int numRaces;
            Console.WriteLine("请输入锦标赛中的比赛数量（3 到 5 场）：");
            while (!int.TryParse(Console.ReadLine(), out numRaces) || numRaces < 3 || numRaces > 5)
            {
                Console.WriteLine("输入错误，请重新输入锦标赛中的比赛数量（3 到 5 场）：");
            }

            for (int raceNumber = 1; raceNumber <= numRaces; raceNumber++)
            {
                Console.WriteLine($"开始第{raceNumber}场比赛。");
                Venue selectedVenue = SelectVenue();
                Console.WriteLine($"本场比赛场地为：{selectedVenue.venueName}，圈数：{selectedVenue.noOfLaps}，平均单圈时间：{selectedVenue.averageLapTime}秒，下雨几率：{selectedVenue.chanceOfRain}%。");
                // 初始化车手比赛时间
                foreach (var driver in drivers.drivers)
                {
                    //GetStartingTimePenalty方法
                    driver.accumulatedTime = GetStartingTimePenalty(driver.Ranking);
                }

                for (int lap = 1; lap <= selectedVenue.noOfLaps; lap++)
                {
                    Console.WriteLine($"第{lap}圈开始。");
                    foreach (var driver in drivers.drivers)
                    {
                        if (driver.eligibleToRace)
                        {
                            int lapTime = selectedVenue.averageLapTime;
                            //故障概率
                            if (RandomNumber.GetRandomValue(1, 100) <= 5)
                            {
                                lapTime += 20; // 轻微机械故障
                                Console.WriteLine($"{driver.name}赛车出现轻微机械故障，单圈时间增加 20 秒。");
                            }
                            else if (RandomNumber.GetRandomValue(1, 100) <= 3)
                            {
                                lapTime += 120; // 重大机械故障
                                Console.WriteLine($"{driver.name}赛车出现重大机械故障，单圈时间增加 120 秒。");
                            }
                            else if (RandomNumber.GetRandomValue(1, 100) <= 1)
                            {
                                driver.eligibleToRace = false;
                                Console.WriteLine($"{driver.name}赛车出现不可恢复的机械故障，退出比赛。");
                            }
                            else
                            {
                                if (driver.specialSkill == "breaking" || driver.specialSkill == "cornering")
                                {
                                    if (lap % 1 == 0)
                                    {
                                        //RandowNumeber类
                                        int timeReduction = RandomNumber.GetRandomValue(1, 8);
                                        lapTime -= timeReduction;
                                        Console.WriteLine($"{driver.name}使用{driver.specialSkill}技能，单圈时间减少{timeReduction}秒。");
                                    }
                                }
                                if (driver.specialSkill == "overtaking" && lap % 3 == 0)
                                {
                                    int timeReduction = RandomNumber.GetRandomValue(10, 20);
                                    lapTime -= timeReduction;
                                    Console.WriteLine($"{driver.name}使用超车技能，单圈时间减少{timeReduction}秒。");
                                }
                            }

                            if (lap == 2 && RandomNumber.GetRandomValue(1, 100) <= selectedVenue.chanceOfRain)
                            {
                                if (RandomNumber.GetRandomValue(1, 2) == 1)
                                {
                                    lapTime += 10;
                                    Console.WriteLine($"{driver.name}在第二圈下雨时更换雨天轮胎，单圈时间增加 10 秒。");
                                }
                            }

                            if (RandomNumber.GetRandomValue(1, 100) <= selectedVenue.chanceOfRain && lap > 2)
                            {
                                lapTime += 5;
                                Console.WriteLine($"{driver.name}在该圈下雨，单圈时间增加 5 秒。");
                            }

                            driver.accumulatedTime += lapTime;
                        }
                    }
                    //对车手累计时间进行排序，FirstOrDefault方法
                    var leadingDriver = drivers.drivers.OrderBy(d => d.accumulatedTime).FirstOrDefault();
                    if (leadingDriver != null)
                    {
                        Console.WriteLine($"第{lap}圈结束，领先车手为：{leadingDriver.name}，时间：{leadingDriver.accumulatedTime}秒。");
                    }
                }
                //获得积分
                AwardPoints();
            }
            //确定冠军
            DetermineChampion();
            //保存车手信息
            SaveDriverDetails();
        }
        //挑选每场的场地，每个场地只能选一次
        private Venue SelectVenue()
        {
            Venue selectedVenue = null;
            while (selectedVenue == null)
            {
                Console.WriteLine("请从以下场地中选择一个：");
                for (int i = 0; i < venues.venues.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {venues.venues[i].venueName}");
                }

                int venueChoice;
                if (!int.TryParse(Console.ReadLine(), out venueChoice) || venueChoice < 1 || venueChoice > venues.venues.Count)
                {
                    Console.WriteLine("输入错误，请重新选择场地。");
                }
                else
                {
                    selectedVenue = venues.venues[venueChoice - 1];
                    venues.venues.RemoveAt(venueChoice - 1);
                }
            }
            return selectedVenue;
        }

        private int GetStartingTimePenalty(int ranking)
        {
            if (ranking == 1) return 0;
            else if (ranking == 2) return 3;
            else if (ranking == 3) return 5;
            else if (ranking == 4) return 7;
            else return 10;
        }
        private void AwardPoints()
        {
            var topDrivers = drivers.drivers.OrderBy(d => d.accumulatedTime).Take(4).ToList();
            for (int i = 0; i < topDrivers.Count; i++)
            {
                //GetPointsForPosition方法
                topDrivers[i].accumulatedScore += GetPointsForPosition(i + 1);
            }
        }

        private int GetPointsForPosition(int position)
        {
            if (position == 1) return 8;
            else if (position == 2) return 5;
            else if (position == 3) return 3;
            else if (position == 4) return 1;
            else return 0;
        }

        private void DetermineChampion()
        {
            var championDriver = drivers.drivers.OrderByDescending(d => d.accumulatedScore).FirstOrDefault();
            //没有冠军车手：championDriver==0
            if (championDriver != null)
            {
                Console.WriteLine($"锦标赛冠军为：{championDriver.name}，总积分：{championDriver.accumulatedScore}。");
            }
            else
            {
                Console.WriteLine("本次锦标赛中没有冠军");
            }
        }

        //保存选手信息
        private void SaveDriverDetails()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter("D:\\P\\Grand_Prix\\Grand_Prix\\data\\champion.txt"))
                {
                    foreach (var driver in drivers.drivers)
                    {
                        writer.WriteLine($"{driver.name},{driver.Ranking},{driver.specialSkill},{driver.eligibleToRace},{driver.accumulatedScore},{driver.accumulatedTime}");
                    }
                }
                Console.WriteLine("车手详细信息已保存到 champion.txt 文件。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"保存文件时出现错误：{ex.Message}");
            }
        }
    }
    class RandomNumber
    {
        private static Random random = new Random();

        public static int GetRandomValue(int min, int max)
        {
            return random.Next(min, max + 1);
        }
    }
}

class Program
{

    static void Main(string[] args)
    {
        ListOfVenues venuesList = new ListOfVenues();
        venuesList.venues = ReadVenuesFromFile("D:\\P\\Grand_Prix\\Grand_Prix\\data\\venues.txt");

        ListOfDrivers driversList = new ListOfDrivers();
        driversList.drivers = ReadDriversFromFile("D:\\P\\Grand_Prix\\Grand_Prix\\data\\driver.txt");

        //接受场地和车手信息并传递给锦标赛类RunChampionship
        Championship championship = new Championship(driversList, venuesList);
        championship.RunChampionship();
    }

    //ReadVenuesFromFile方法
    private static List<Venue> ReadVenuesFromFile(string fileName)
    {
        List<Venue> venues = new List<Venue>();
        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] venueDetails = line.Split(',');
                    if (venueDetails.Length == 4)
                    {
                        Venue venue = new Venue
                        {
                            venueName = venueDetails[0],
                            noOfLaps = int.
                            TryParse(venueDetails[1], out int laps) ? laps : 0,
                            averageLapTime = int.TryParse(venueDetails[2], out int time) ? time : 0,
                            chanceOfRain = double.TryParse(venueDetails[3], out double chance) ? chance : 0,
                        };
                        venues.Add(venue);
                    }
                    else
                    {
                        Console.WriteLine($"无效场地数据行.{line}");
                    }
                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"读取场地文件时出现错误：{ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取场地文件时出现错误：{ex.Message}");
        }
        return venues;
    }

    private static List<Driver> ReadDriversFromFile(string fileName)
    {
        List<Driver> drivers = new List<Driver>();
        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] driverDetails = line.Split(',');


                    if (driverDetails.Length == 5)
                    {
                        Driver driver = new Driver
                        {

                            name = driverDetails[0],
                            Ranking = int.Parse(driverDetails[1]),
                            specialSkill = driverDetails[2],
                            eligibleToRace = true,
                            accumulatedScore = 0
                        };

                        drivers.Add(driver);
                    }
                    else
                    {
                        Console.WriteLine($"无效车手数据行.{line}");
                    }

                }
            }
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"文件未找到: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"读取车手文件时出现其他错误: {ex.Message}");
        }
        return drivers;

    }
};