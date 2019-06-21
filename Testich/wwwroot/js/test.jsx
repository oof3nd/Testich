import { Component } from 'react';
import {
    Link,
    BrowserRouter,
    Route,
    Switch,
    StaticRouter,
    Redirect,
} from 'react-router-dom';



export default class TestComponent extends Component {
    constructor(props) {
        super(props);
        this.state = {
            lvls: this.props.lot, //Уровни
            lvlHelp: this.props.lot[0].solution, //Подсказка уровня
            currentLvl: this.props.lot[0], //Тек. уровень
            nextLvl: this.props.lot[1], //След. уровень
            qId: this.props.qId, //айди вопроса
            numbercq: this.props.numbercq, //Номер вопроса
            contentcq: this.props.contentcq, // Содержание вопроса
            selectedOption: null, // Выбранный ответ
            correctOption: false, // Проверка на правильность ответа
            cqos: this.props.cqos, //Ответы к вопросу
            resultTest: false //Проверка на вывод результата
    };

        this.loadData = []; //Данные, кот. приходят с post
        this.lvlsValues = [...this.props.lot]; //Уровни для UI и проверки
        this.answers = []; //Ответы на вопросы
        this.persentOptions = 0; //
       // this.props.lot[0].statusCorrect = true;
    }

    handleOptionChange = changeEvent => {
        this.setState({
            selectedOption: parseInt(changeEvent.target.value)
        });
       // console.log(changeEvent.target.value);

    };

    handleFormSubmit = formSubmitEvent => {
        formSubmitEvent.preventDefault();

        if (this.answers.length > 0) {
            let correctOptions = this.answers.filter(answer => answer.correctOption == true).length;
            let uncorrectOptions = this.answers.filter(answer => answer.correctOption == false).length;

            if (uncorrectOptions > 0 || correctOptions > 0) {
                this.persentOptions = correctOptions - uncorrectOptions + 1;
                console.log("per = " + this.persentOptions);
            }

        }

        var data = JSON.stringify({
            "TestId": this.props.id,
            "Level": this.state.currentLvl,
            "LevelNext": this.state.nextLvl,
            "QuestionId": this.state.qId,
            "QuestionIndexNumber": this.state.numbercq,
            "SelectedOption": this.state.selectedOption,
            "CorrectOption": this.state.correctOption,
            "PersentOptions": this.persentOptions,
            "MaxLvls": this.props.lot.length,
        });

        var xhr = new XMLHttpRequest();
        var url = "/Tests/Testing";

        xhr.open("post", url, true);
        xhr.setRequestHeader("Content-type", "application/json");
        xhr.send(data);

        xhr.onreadystatechange = () => {
      
            if (xhr.readyState === 4 && xhr.status === 200) {

                this.loadData = JSON.parse(xhr.responseText);
                this.setState({ correctOption: this.loadData.CorrectOption, selectedOption: null });

                let obj = new Object();
                obj.levelIndexNumber = this.state.currentLvl.levelIndexNumber;
                obj.numbercq = this.state.numbercq;

                obj.contentcq = this.state.contentcq;
                obj.correctOption = this.state.correctOption;
                this.answers.push(obj);

                if (this.state.correctOption) {

                    let lvl = this.lvlsValues.shift();
                    let newLvls = [...this.props.lot];
                    let indexLvl = newLvls.findIndex(x => x.id === lvl.id);
                    newLvls[indexLvl].statusCorrect = true;
                    this.setState({ lvls: newLvls });

                    if (this.lvlsValues.length > 1) {

                        this.setState({
                            qId: this.loadData.QuestionId,
                            numbercq: this.loadData.QuestionIndexNumber,
                            contentcq: this.loadData.QuestionContent,
                            cqos: this.loadData.Cqos
                        });
                        this.setState({
                            currentLvl: this.lvlsValues[0],
                            nextLvl: this.lvlsValues[1],
                            lvlHelp: this.lvlsValues[0].solution
                        });

                    }
                    else {

                        this.setState({
                            qId: this.loadData.QuestionId,
                            numbercq: this.loadData.QuestionIndexNumber,
                            contentcq: this.loadData.QuestionContent,
                            cqos: this.loadData.Cqos
                        });

                        this.setState({
                            currentLvl: this.loadData.LevelNext,
                            lvlHelp: this.loadData.LevelNext.solution
                        });

                        this.setState({ resultTest: this.loadData.ResultTest });
                    }

                    this.setState({
                        correctOption: false
                    });

                }
                else {

                    this.setState({ qId: this.loadData.QuestionId, numbercq: this.loadData.QuestionIndexNumber, contentcq: this.loadData.QuestionContent, cqos: this.loadData.Cqos});
                }
            }
            else
            {
              //  console.log("error = " + this.status);
            }
        };


    };


    render() {

        const cqos = this.state.cqos.map(cqo => <div className="custom-control custom-radio mt-1">
            <input type="radio" className="custom-control-input" key={cqo.optionNumber} id={cqo.optionNumber} value={cqo.optionNumber} name="cqosCheckBox" checked={this.state.selectedOption === cqo.optionNumber} onChange={this.handleOptionChange} />
            <label className="custom-control-label" for={cqo.optionNumber}>{cqo.content}</label>
        </div>
        );

        const answersUI = this.answers.map(answer =>
            <tr>
                <td>{answer.levelIndexNumber}</td>
                <td>{answer.numbercq}</td>
                <td>{answer.contentcq}</td>
                {answer.correctOption ? <td className="table-success"> Да </td> : <td className="table-danger">Нет</td>}
            </tr>
        );

       // console.log(this.answersUI);

        const app = (

            <div>
            {!this.state.resultTest ? 
                <div className="container">
                    <div className="jumbotron">

                    <InfoTest id={this.props.id} des={this.props.des} time={this.props.time} />

                            <Levels lot={this.state.lvls} lvlHelp={this.state.lvlHelp} time={this.props.time} />
                    

                    <Switch>
                        <Route
                            exact
                            path="/"
                            render={() => <Redirect to="/Tests/Testing/" />}
                        />

                        <Route path="/Tests/Testing/"/>

                        <Route
                            path="*"
                            component={({ staticContext }) => {
                                if (staticContext) staticContext.status = 404;

                                return <h1>Not Found :(</h1>;
                            }}
                        />
                    </Switch>

                </div>
                    <form onSubmit={this.handleFormSubmit}>
                        <div className="jumbotron">

                            <Question numbercq={this.state.numbercq} contentcq={this.state.contentcq} /> 

                            {cqos}

                        </div>

                        <button type="submit" className="btn btn-success btn-lg btn-block" disabled={!this.state.selectedOption}>Ответить</button>
                    </form>
                </div>
                    :
                <div className="container">
                    <table class="table">
                        <thead>
                            <tr>
                                <th scope="col">Уровень</th>
                                <th scope="col">Вопрос</th>
                                <th scope="col">Содержание</th>
                                <th scope="col">Результат</th>
                            </tr>
                        </thead>
                        <tbody>
                            {answersUI}
                        </tbody>
                        </table>
                        <a class="btn btn-primary btn-sm" role="button" href="/Tests/MyResults">Мои результаты</a>
                </div>
                
            }
            </div>
        );

        if (typeof window === 'undefined') {
            return (
                <StaticRouter
                    context={this.props.context}
                    location={this.props.location}
                >
                    {app}
                </StaticRouter>
            );
        }
        return <BrowserRouter>{app}</BrowserRouter>;
    }
}

class InfoTest extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
        <div>
            <h3>
                Тест № {this.props.id}
            </h3>
            <h6>
                Описание: {this.props.des}
            </h6>
            <h6>
                Время на прохождение: {this.props.time} мин.
           </h6>
               
        </div>
        );
    }
}

class Levels extends Component {

    state = {
        showHelp: false
    };

    
    handleHelpChange = () => {
        this.setState({
            showHelp: !this.state.showHelp
        });
     //   console.log(this.state.showHelp);
    };

    
    render() {
        const uiLvls = this.props.lot.map(lvl => <li className={lvl.statusCorrect ? 'list-group-item list-group-item-success' : 'list-group-item list-group-item-secondary'} key={lvl.id}>
            {lvl.levelIndexNumber }
        </li>);

        return (
            <div>

                <TimerApp time={this.props.time} />

                <nav aria-label="Уровни теста">
                    <ul className="list-group list-group-horizontal-sm">
                        <li className="list-group-item list-group-item-light">
                            Уровни
                        </li>
                        {uiLvls}
                    </ul>
                </nav>
            <h4>
            </h4>

                <button className="btn btn-warning btn-sm mt-1" data-dismiss="alert" id="btn-solution" onClick={this.handleHelpChange}>Подсказка уровня</button>
                {
                    this.state.showHelp ?
                    <div>
                            <div className="alert alert-warning mt-2" role="alert">
                                {this.props.lvlHelp}
                            </div>
                    </div>
                    : null
                }
            </div>
        );
    }
}

class Question extends Component {


    render() {
      
        return (
            <div>
                <ul className="list-group">
                    <li className="list-group-item active">Вопрос № {this.props.numbercq}</li>
                    <li className="list-group-item text-break">Описание вопроса: {this.props.contentcq}</li>
                </ul>
            </div>
        );
    }
}

class ClosedQuestionOptions extends Component {
    constructor(props) {
        super(props);
    }

  

    render() {


    }
}

class TimerInput extends Component {
    render() {
        return (
            <div style={{ marginLeft: 100 }}>
                <h3>Input your desired time</h3>
                <input type="number" value={this.props.value} onChange={this.props.handleChange} required />
            </div>
        );
    }
}

class Timer extends Component {
    render() {
        return (
            <div>
                <h6>Времени осталось:  <span class="badge badge-light">{this.props.value}:{this.props.seconds} </span></h6>
            </div>
        );
    }
}

class StartButton extends Component {
    render() {
        return (
            <div style={{ marginLeft: 130 }}>
                <button className="btn btn-lg btn-success" disabled={!this.props.value} onClick={this.props.startCountDown}>Start</button>
            </div>

        );
    }
}

class TimerApp extends Component {
    constructor(props) {
        super(props);
        this.state = {
            seconds: '00',
            value: this.props.time,
            isClicked: false
        }
        this.secondsRemaining;
        this.intervalHandle;
        this.handleChange = this.handleChange.bind(this);
        this.startCountDown = this.startCountDown.bind(this);
        this.tick = this.tick.bind(this);
    }

    componentDidMount() {
        this.intervalHandle = setInterval(this.tick, 1000);
        let time = this.state.value;
        this.secondsRemaining = time * 60;
        this.setState({
            isClicked: true
        })
    }

    handleChange(event) {
        this.setState({
            value: event.target.value
        })
    }

    tick() {
        var min = Math.floor(this.secondsRemaining / 60);
        var sec = this.secondsRemaining - (min * 60);

        this.setState({
            value: min,
            seconds: sec,
        })

        if (sec < 10) {
            this.setState({
                seconds: "0" + this.state.seconds,
            })

        }

        if (min < 10) {
            this.setState({
                value: "0" + min,
            })

        }

        if (min === 0 & sec === 0) {
            clearInterval(this.intervalHandle);
        }


        this.secondsRemaining--
    }

    startCountDown() {
        //this.intervalHandle = setInterval(this.tick, 1000);
        //let time = this.state.value;
        //this.secondsRemaining = time * 60;
        //this.setState({
        //    isClicked: true
        //})
    }

    render() {
        const clicked = this.state.isClicked;
        if (clicked) {
            return (
                <div>
                    <Timer value={this.state.value} seconds={this.state.seconds} />
                </div>
            );
        } else {
            return (
                <div>
                    {/*    <TimerInput value={this.state.value} handleChange={this.handleChange} /> */}
                    <Timer value={this.state.value} seconds={this.state.seconds} />
                    {/*  <StartButton startCountDown={this.startCountDown} value={this.state.value} /> */}
                </div>
            );
        }
    }
}