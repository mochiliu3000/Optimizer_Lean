from datetime import datetime, date
from zipfile import ZipFile
import os

'''				
datestimes,open,high,low,close,volume				
14400000,1689500,1689800,1689500,1689800,800
14460000,1690000,1690500,1690000,1690100,12300

datestimes,volrmb,open,high,low,close,avgprice,bid,ask
2011-08-08 14:46:00,2174.0000,33482.0000,33505.0000,33478.0000,33487.0000,0.0000,0.0000,0.0000
2011-08-08 14:47:00,2136.0000,33487.0000,33501.0000,33482.0000,33501.0000,0.0000,0.0000,0.0000
'''
# refer to https://www.quantconnect.com/lean/documentation/topic16.html
def txt_to_csv(txt_file_dir, csv_folder):	
	with open(txt_file_dir, 'r') as txt_file:
		next(txt_file)
		for line in txt_file:
			# process the line
			columns = line.split(',')
			time = datetime.strptime(columns[0], '%Y-%m-%d %H:%M:%S')
			seconds = (time - datetime(1970, 1, 1)).total_seconds()
			day_seconds = (time.date() - date(1970, 1, 1)).total_seconds()
			new_line = "%.0f,%.0f,%.0f,%.0f,%.0f,%.0f\n" % ((seconds - day_seconds) * 1000, float(columns[2]) * 10000, float(columns[3]) * 10000, float(columns[4]) * 10000, float(columns[5]) * 10000, float(columns[1]))

			filename = csv_folder + "/" + time.date().strftime('%Y%m%d') + "_spy_minute_trade.csv"
			with open(filename, 'a+') as csv_file:
				csv_file.write(new_line)


txt_file_dir = "./RU.1min.2011.08.08.14.46.00.2017.02.10.14.59.00.txt"
csv_file_dir = "./csv"
txt_to_csv(txt_file_dir, csv_file_dir)

def csv_to_zip(csv_folder, zip_folder):
	for filename in os.listdir(csv_folder):
		zipname = zip_folder + '/' + filename[:8] + '_trade.zip'
		with ZipFile(zipname, 'w') as myzip:
			myzip.write(csv_folder + '/' + filename, filename)

zip_file_dir = "./zip"
csv_to_zip(csv_file_dir, zip_file_dir)

