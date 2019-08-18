import pandas as pd
from flask import Flask, request, jsonify
import pickle
import numpy as np
import pandas as pd

app = Flask(__name__)

# load the Linear Regression model from Pickle file that we exported it to
#model = pickle.load(open('LogicUniversity_LinReg.pkl', 'rb'))
model = pickle.load(open('LogicUniversity_KNN.pkl', 'rb'))


# handles the POST request to this home URL
@app.route('/', methods=['POST'])
def apicall():
    req = request.get_json(force = True) # get entire incoming request
    length = len(req)
    prediction= np.zeros(length)
    for x in range(0,length):
        prediction[x] = model.predict([[req[x]['ItemCode'],req[x]['Month']]])
    result=pd.Series(prediction).to_json(orient='values')
    #prediction = model.predict([[req[2]['ItemCode'],req[2]['Year'],req[2]['Month']]])
    #result = prediction[0]
    
    
    #prediction = model.predict([[req[0]['X']]]) # extract the 'X' property from incoming request and do prediction
    #result = prediction[0] # get the result from the result array
    
    
    #yyy=np.full((5, 1), 100)
    #yyy[1][0]=10000
    #prediction = model.predict(yyy)
    #result=pd.Series(prediction).to_json(orient='values')
    return jsonify(result)

# run the server
if __name__ == '__main__':
    app.run(port=5000, debug=True)