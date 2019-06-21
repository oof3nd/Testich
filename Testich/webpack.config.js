/// <binding BeforeBuild='Run - Development, Run - Production' />
const path = require('path');
module.exports = {
        entry: {
            app: './wwwroot/js/run.js',
        },
        output: {
            filename: '[name].js',
            globalObject: 'this',
            path: path.resolve(__dirname, 'wwwroot/dist'),
        },
        module: {
            rules: [
                {
                    test: /\.jsx?$/,
                    exclude: /node_modules/,
                    loader: 'babel-loader',
                },
            ],
        },
    };