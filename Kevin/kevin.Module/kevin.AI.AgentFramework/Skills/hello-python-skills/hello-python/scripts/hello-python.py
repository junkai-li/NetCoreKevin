import sys
def process_data(var1): 
    for i in var1[1:len(var1)]:
         print( f'参数: {i}\n')
    print(f'Hello,Python ')
if __name__ == '__main__':
    # 从命令行参数获取数据 
    process_data(sys.argv)