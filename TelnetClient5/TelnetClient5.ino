#include <SoftwareSerial.h>             // подключаем библиотеку SoftwareSerial
SoftwareSerial RfidReader(3, 4);        // определяем RFID-модуль (RX,TX)
#include <SPI.h>
#include <Ethernet2.h>
#include <Wire.h>                       // include protocol "I2C"
#include <LiquidCrystal_I2C.h>
LiquidCrystal_I2C lcd( 0x27,16,2);      //МЭЛТ адрес:0x3F ; неМЭЛТ адрес:0x27

int num;                                // назначаем переменную для значений номера карты
int room = 403;                         // назначаем номер кабинета, где установлен валидатор
int count = 0;                          // назначаем переменную для счетчика
int Card_Num[14];       // назначаем переменную для номера магнитной карты  
int Card_Num_Buf[14];       // назначаем буферную переменную для номера магнитной карты  
bool New_Card = false; // назначаем флаг для новой магнитной карты  
bool End_Reed = false; // назначаем флаг окончания чтения магнитной карты


// Enter a MAC address and IP address for your controller below.
// The IP address will be dependent on your local network:
byte mac[] = {0x4C, 0xED, 0xFB, 0x3E, 0xDB, 0xED};// mac адрес шилда ethernet arduino (присваивается принудительно)
IPAddress ip(192, 168, 1, 177); //IP Address шилда ethernet arduino

// Enter the IP address of the server you're connecting to:
IPAddress server(192, 168, 1, 138);
const char* host = "192.168.1.138";       // Локальный адрес сервера - читается с#

// Initialize the Ethernet client library
// with the IP address and port of the server
// that you want to connect to (port 23 is default for telnet;
// if you're using Processing's ChatServer, use  port 10002):
EthernetClient client;

unsigned long lastConnectionTime = 0;             // last time you connected to the server, in milliseconds
const unsigned long postingInterval = 10L * 1000L; // delay between updates, in milliseconds
// the "L" is needed to use long type numbers
String HTTP_req; // для хранения HTTP запроса
String FDStr; // для хранения ответа HTTP запроса First Display String - Surname + Name// 
String SDStr; // для хранения ответа HTTP запроса Second Display String - "in"/ "out"/ "Net v BD": "15:40"// 
unsigned long lastTRNewCard = 0;             // last time, when you received new card
const unsigned long TRNewCard = 3L * 1000L; // delay between updates, in milliseconds
bool delayRepeatCard = false;

void setup() {
  lcd.init();
  lcd.backlight();
  lcd.setCursor(4,0);
  lcd.print ("Hello!!!");
  HTTP_req.reserve(150); // Резервирование памяти - было 100, 200 не вытягивает коннект
  FDStr.reserve(16); // Резервирование памяти
  SDStr.reserve(16); // Резервирование памяти
  Ethernet.begin(mac, ip);              // start the Ethernet connection
  Serial.begin(9600);                   // Open serial communications and wait for port to open
  RfidReader.begin(9600);               // инициируем передачу данных в последовательный порт, к которому подключен модуль


//  while (!Serial) {
//    ; // wait for serial port to connect. Needed for Leonardo only
//  }

  // give the Ethernet shield a second to initialize:
  delay(1000);
  Serial.println("Setup stage: connecting...");
  lcd.clear();
  lcd.setCursor(0,0);
  lcd.print ("Connecting...");

  // if you get a connection, report back via serial:
  if (client.connect(server, 80)) {
    Serial.println("Setup stage: connected");
    Serial.print("Setup  stage: Local IP client: ");
    Serial.println(Ethernet.localIP());
    lcd.clear();
    lcd.setCursor(0,0);
    lcd.print ("Connected");
    lcd.setCursor(0,1);
    lcd.print ("IP:");
    lcd.print (Ethernet.localIP());
  }
  else {
    // if you didn't get a connection to the server:
    Serial.println("ARDU Setup: connection failed");
    lcd.clear();
    lcd.setCursor(0,0);
    lcd.print ("Connect failed");
  }
    lcd.noBacklight();
}

void loop()
{
 /*считываение карты начало*/ 
  while (RfidReader.available() > 0)                 // странно - этот цикл должен быть вечным, если не отвдить карту, но цикл прерывается и запись о новой карте выводится в порт. Проверить это позже.
    { 
        Card_Num_Buf[count] =  RfidReader.read();    // считываем значение
        End_Reed = false;                            // чтение карты началось сначала
        if (Card_Num_Buf[count] != Card_Num[count])  // если карта другая (речь об одной ячйке)
        {
          Card_Num[count] = Card_Num_Buf[count];     // сохраняем другую (новую) карту (речь об одной ячйке)
          New_Card = true;                           // карта новая  
        }
        //Serial.print(Card_Num_Buf[count]);         // тестовый вывод
        count++;                                     // увеличиваем счетчик
        if (count >= 14)                             // если количество чисел больше 14, то
        {                  
         //Serial.println(' ');                      // переходим на новую строку, тестовый вывод
         count = 0;                                  // обнуляем счетчик
         End_Reed = true;                            // карта прочитaна до конца
         } else {                                    // если чисел меньше, то
          //Serial.print(", ");                      // выводим числа одно за другим через запятую и пробел, тестовый вывод
         }
    }

  if (New_Card == true && End_Reed == true)          // если карта новая и прочитана полностью
  {
    lcd.backlight();
    Serial.println("");                              // выводим на новую строку
    Serial.println("New Card:");                     // выводим запись о том, что карта новая
    for (int i=0; i < 14; i++)                       // выводим данные по карте в порт из массива
    {
      Serial.print(Card_Num[i]);
      Serial.print(", "); 
    }
    Serial.println(' ');
    httpRequest();//отправляем запрос на сервер
    New_Card = false;                                 // теперь карта не новая
    End_Reed = false;                                 // чтение карты можно начинать сначала
    lastTRNewCard = millis();                         // запускаем с нуля таймер времени последнего приложения карты
    delayRepeatCard = true;                           // задержка на обнуление прочитанной карты включена
  }
  /*считываение карты конец*/  

  if ((millis() - lastTRNewCard > TRNewCard) && (delayRepeatCard)) {
    for (int i=0; i < 14; i++) {                       // стираем карту для возможности повторного приложения
      Card_Num[i]= 0;
      }
      delayRepeatCard = false;                         // задержка на обнуление прочитанной карты больше не нужна
      
      lcd.clear();                                     // стираем данные с дисплея
      lcd.setCursor(4,0);
      lcd.print ("Attach" );
      
      lcd.setCursor(4,1);
      lcd.print ("a card");
//      analogWrite(BACKLIGHT, LCD_BRIGHT_MIN);
      lcd.noBacklight();
   }
  
  // if there are incoming bytes available
  // from the server, read them and print them:
  if (client.available()) {
    char c = client.read();
    Serial.print(c);
    HTTP_req += c; // сохраняем символ HTTP запроса
    if (HTTP_req.indexOf("endssp") > -1 ){
      httpRequestParse();
      Serial.println("");                              // выводим на нову строку    
      Serial.println("Result of parsing request:");
      Serial.println("FDStr = " + FDStr);
      Serial.println("SDStr = " + SDStr);

      lcd.backlight();       
      lcd.clear();
      lcd.setCursor(0,0);
      lcd.print (FDStr);
      
      lcd.setCursor(0,1);
      lcd.print (SDStr);
    }

  }
}

void httpRequest()
{
  // close any connection before send a new request.
  // This will free the socket on the WiFi shield
  client.stop();
  
  if (client.connect(server, 80)) 
  {
    Serial.println("");                              // выводим на нову строку
    Serial.print("ARDU Req: Соединение с IP:");
    Serial.println("192,168,1,138");
    Serial.print("ARDU send: ");
    client.print("GET /index?");
    client.print("num=");
    for (int i=0; i < 14; i++)      // выводим данные по карте в порт из массива
    {
      client.print(Card_Num[i]);
      Serial.print(Card_Num[i]);
    }
	  client.print("&room=");
	  client.print(room);
    client.println(" HTTP/1.1"); //удаление этой строчки прерывает передачу
    client.print( "Host: " );//удаление этой строчки прерывает передачу
    client.println(host);//удаление этой строчки прерывает передачу
    client.println( "Connection: close" );//удаление этой строчки прерывает передачу
    client.println();//удаление этой строчки прерывает передачу
    client.println();//удаление этой строчки прерывает передачу 
     // note the time that the connection was made:
    //lastConnectionTime = millis();   
  }
  else 
  {
    // if you couldn't make a connection:
    Serial.println("ARDU Req: connection failed");
    lcd.clear();
    lcd.setCursor(0,0);
    lcd.print ("Connect failed");
  }
}

void httpRequestParse(){
   if (HTTP_req.indexOf("endssp") > -1)
   {
    byte iBegin = HTTP_req.indexOf("FDStr");
    byte iEnd = HTTP_req.indexOf(",EFDStr,");
    FDStr = HTTP_req.substring(iBegin+8,iEnd);

    iBegin = HTTP_req.indexOf("SDStr");
    iEnd = HTTP_req.indexOf(",ESDStr,");
    SDStr = HTTP_req.substring(iBegin+8,iEnd);
   }

   HTTP_req = ""; // очищаем строку запроса
}
