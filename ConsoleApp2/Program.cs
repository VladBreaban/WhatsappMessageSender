// See https://aka.ms/new-console-template for more information

Console.WriteLine("Welcome to CraftedForEveryone.com");

//ChromeWhatsAppSender asd = new ChromeWhatsAppSender();
//asd.SendTextMessage("40730747931", "ha");
new ChromeWhatsAppSender().AttachFileWithCustomMessage("+40730747931", @"C:\Users\Vlad Manole\Downloads\CV Jercan Adrian Mihai.pdf", "hello");