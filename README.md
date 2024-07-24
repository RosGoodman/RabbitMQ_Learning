# RabbitMQ_Learning

Код адаптирован из книги "RabbitMQ Essentials" - David Dossot.
Репозиторий с примерами на Java: https://github.com/tolyo/rabbitmq-essentials/tree/master

1. Установить erlang https://www.erlang.org/downloads
2. Скачать и запустить инсталлер https://www.rabbitmq.com/docs/download
3. Подключить плагины.
Через командную строку открыть папку с установленной программой RabbitMQ Server/rabbitmq_server-3.13.5/sbin
Выполнить команду: rabbitmq-plugins enable rabbitmq_management (см. https://www.rabbitmq.com/docs/management)
4. Проверить подключение http://localhost:15672/
логин:  guest
пароль: guest

5. Для нового пользователя:
rabbitmqctl add_user username password
rabbitmqctl set_user_tags userTag
rabbitmqctl set_permissions admin ".*" ".*" ".*"