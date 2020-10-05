﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace OOPExamples
{
    class Program
    {
        private const int BANKS_MIN = 2;
        private const int BANKS_MAX = 5;

        private const int NAME_MAX_LENGTH = 10;
        private const int NAME_MIN_LENGTH = 4;
        static void Main(string[] args)
        {
            var banks = GenerateBanks();
            var users = GenerateUsers(banks);
            int i = 0;

            //1) Сделать выборку всех Пользователей, имя + фамилия которых длиннее чем 12 символов.
            var users12 = users.Where(x => x.FirstName.Length + x.LastName.Length > 12);
            foreach (User user in users12)
                Console.WriteLine($"{user.FirstName} {user.LastName} |{user.FirstName.Length + user.LastName.Length}");

            //2) Сделать выборку всех транзакций (в результате должен получится список из 1000 транзакций)
            var bankTrans = banks.SelectMany(x => x.Transactions);
            
            foreach (var trans in bankTrans)
            {
                i++;
                Console.WriteLine($"{i}) {trans.Value} {trans.Currency}");
            }

            //3) Вывести Банк: и всех его пользователей (Имя + фамилия + количество транзакий в гривне) отсортированных по Фамилии по убиванию. в таком виде :
            //   Имя банка 
            //   ***************
            //   Игорь Сердюк 
            //   Николай Басков

            foreach (Bank bank in banks)
            {
                var usersBank = users.Where(x => x.Bank.Name == bank.Name).OrderByDescending(x => x.LastName);
                Console.WriteLine($"\n{bank.Name}\n***************");

                foreach (User user in usersBank)
                {
                    var userTrans = user.Transactions.Where(x => x.Currency == Currency.UAH).Count();
                    Console.WriteLine($"{user.FirstName} {user.LastName} | {userTrans}");
                }
            }


            //4) Сделать выборку всех Пользователей типа Admin, у которых счет в банке, в котором больше всего транзакций

            Bank bankMaxTrans = banks[0];
            foreach (Bank bank in banks)
            {
                if (bank.Transactions.Count > bankMaxTrans.Transactions.Count)
                    bankMaxTrans = bank;
            }

            var BankAdmins = users.Where(x => x.Type == UserType.Admin && x.Bank == bankMaxTrans);
            foreach (var bankAdmins in BankAdmins)
                Console.WriteLine($"{bankAdmins.Type} | {bankAdmins.Bank.Name} | {bankAdmins.FirstName} {bankAdmins.LastName}");

            //5) Найти Пользователей(НЕ АДМИНОВ), которые произвели больше всего транзакций в определенной из валют (UAH,USD,EUR) 
            //то есть найти трёх пользователей: 1й который произвел больше всего транзакций в гривне, второй пользователь, который произвел больше всего транзакций в USD 
            //и третьего в EUR

            User userMaxCountTransUAH = users[0];
            User userMaxCountTransEUR = users[0];
            User userMaxCountTransUSD = users[0];

            foreach (User user in users)
            {
                if (user.Type != UserType.Admin)
                {
                    var CountTransUAH = user.Transactions.Where(x => x.Currency == Currency.UAH).Count();
                    if (CountTransUAH > userMaxCountTransUAH.Transactions.Where(x => x.Currency == Currency.UAH).Count())
                        userMaxCountTransUAH = user;
                    var CountTransEUR = user.Transactions.Where(x => x.Currency == Currency.EUR).Count();
                    if (CountTransEUR > userMaxCountTransEUR.Transactions.Where(x => x.Currency == Currency.EUR).Count())
                        userMaxCountTransEUR = user;
                    var CountTransUSD = user.Transactions.Where(x => x.Currency == Currency.USD).Count();
                    if (CountTransUSD > userMaxCountTransUSD.Transactions.Where(x => x.Currency == Currency.USD).Count())
                        userMaxCountTransUSD = user;
                }
            }
            Console.WriteLine($"User Max Count Trans UAH: {userMaxCountTransUAH.Id}) {userMaxCountTransUAH.FirstName} {userMaxCountTransUAH.LastName}");
            Console.WriteLine($"User Max Count Trans EUR: {userMaxCountTransEUR.Id}) {userMaxCountTransEUR.FirstName} {userMaxCountTransEUR.LastName}");
            Console.WriteLine($"User Max Count Trans USD: {userMaxCountTransUSD.Id}) {userMaxCountTransUSD.FirstName} {userMaxCountTransUSD.LastName}");


            //6) Сделать выборку транзакций банка, у которого больше всего Pemium пользователей

            var premiumUsers = users.Where(x => x.Type == UserType.Pemium);
            int[] banks1 = new int[banks.Count];
            Bank premiumBank = banks[0];

            foreach (User user in premiumUsers)
                banks1[user.Bank.Id - 1] = banks1[user.Bank.Id - 1] + 1;

            for (i = 1; i <= banks1.Length - 1; i++)
            {
                if (banks1[i - 1] > banks1[i])
                    premiumBank = banks[i - 1];
            }

            foreach (Transaction transaction in premiumBank.Transactions)
                Console.WriteLine($"{transaction.Value} {transaction.Currency}");

        }

        public class User
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public List<Transaction> Transactions { get; set; }
            public UserType Type { get; set; }
            public Bank Bank { get; set; }
        }

        public class Transaction
        {
            public decimal Value { get; set; }

            public Currency Currency { get; set; }
        }

        public static List<Transaction> GetTenTransactions()
        {
            var result = new List<Transaction>();
            var sign = random.Next(0, 1);
            var signValue = sign == 0 ? -1 : 1;
            for (var i = 0; i < 10; i++)
            {
                result.Add(new Transaction
                {
                    Value = (decimal)random.NextDouble() * signValue * 100m,
                    Currency = GetRandomCurrency(),
                });
            }

            return result;
        }

        public class Bank
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public List<Transaction> Transactions { get; set; }
        }

        public enum UserType
        {
            Default = 1,
            Pemium = 2,
            Admin = 3
        }

        public static UserType GetRandomUserType()
        {
            int userTypeInt = random.Next(1, 4);

            return (UserType)userTypeInt;
        }

        public enum Currency
        {
            USD = 1,
            UAH = 2,
            EUR = 3
        }

        public static Currency GetRandomCurrency()
        {
            int userTypeInt = random.Next(1, 4);

            return (Currency)userTypeInt;
        }

        public static List<Bank> GenerateBanks()
        {
            var banksCount = random.Next(BANKS_MIN, BANKS_MAX);
            var result = new List<Bank>();

            for (int i = 0; i < banksCount; i++)
            {
                result.Add(new Bank
                {
                    Id = i + 1,
                    Name = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Transactions = new List<Transaction>()
                });
            }

            return result;
        }

        public static List<User> GenerateUsers(List<Bank> banks)
        {
            var result = new List<User>();
            int bankId = 0;
            Bank bank = null;
            List<Transaction> transactions = null;
            for (int i = 0; i < 100; i++)
            {
                bankId = random.Next(0, banks.Count);
                bank = banks[bankId];
                transactions = GetTenTransactions();
                result.Add(new User
                {
                    Bank = bank,
                    FirstName = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Id = i + 1,
                    LastName = RandomString(random.Next(NAME_MIN_LENGTH, NAME_MAX_LENGTH)),
                    Type = GetRandomUserType(),
                    Transactions = transactions
                });
                bank.Transactions.AddRange(transactions);
            }

            return result;
        }

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}