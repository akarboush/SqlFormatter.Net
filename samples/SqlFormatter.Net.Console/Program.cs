using SqlFormatter.Net.Languages;

const string input = @"select supplier_name,city from (select * from suppliers join addresses on suppliers.address_id=addresses.id)
as suppliers
where supplier_id>500
order by supplier_name asc,city desc;";


var formatedQ = new MariaDbFormatter().Format(input);

Console.WriteLine(formatedQ);
