#HtmlToPdfConverter

Краткий алгоритм работы:
1. Файл пользователя предварительно загружается в базу данных с целью защиты от перезапусков сервиса.
2. Запускается Hangfire job для конвертации файла.
3. Опрос на стороне клиента прогресса по Hangfire job (pull model).
4. Выходной файл решено было дополнительно защитить и предварительно сохранить в бд.
4. После успешной конвертации пользователю доступна ссылка на скачивание файла.
5. Когда пользователь отправляет запрос на скачивание файла, проверяется наличие локальной копии, если её нет, то файл подгружается из бд.

Данная архитектура позволяет горизонтально масштабировать микросервис, без проблем с отсутствием локальной копии файлов. В случае падения микросервиса, Hangfire автоматически перезапустит job на другом доступном сервере.
