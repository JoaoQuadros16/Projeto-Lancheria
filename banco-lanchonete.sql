create database lanchonete;

use lanchonete;

create table produtos(
id_produto int auto_increment primary key,
nome varchar (120),
preco float, 
categoria varchar(120)
);